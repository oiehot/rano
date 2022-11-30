#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rano.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace Rano.Database.CloudSync
{
    /// <summary>
    /// 로컬 데이터베이스와 Firestore 클라우드 데이터베이스를 싱크해준다.
    /// </summary>
    public sealed class FirestoreSyncManager : ManagerComponent, ICloudSyncManager
    {
        private enum EState
        {
            None = 0,
            Initializing,
            Ready,
            Syncing
        }

        private EState _state = EState.None;
        private ILocalDatabase? _local;
        private ICloudDatabase? _cloud;
        private IAuthManager? _auth;

        public bool IsInitialized => _state >= EState.Ready;
        
        /// <summary>
        /// 초기화 한다.
        /// </summary>
        /// <param name="localDatabase">로컬 데이터베이스</param>
        /// <param name="cloudDatabase">클라우드 데이터베이스</param>
        /// <param name="authManager">UserId를 얻기 위해서 필요하다</param>
        /// <returns>초기화 결과</returns>
        public bool Initialize(ILocalDatabase localDatabase, ICloudDatabase cloudDatabase, IAuthManager authManager)
        {
            Debug.Assert(localDatabase != null);
            Debug.Assert(cloudDatabase != null);
            Debug.Assert(authManager != null);
            
            Log.Info(Constants.INITIALIZING);
            
            if (localDatabase.IsInitialized == false)
            {
                Log.Warning(Constants.LOCAL_DATABASE_NOT_INITIALIZED);
                return false;
            }
            
            if (cloudDatabase.IsInitialized == false)
            {
                Log.Warning(Constants.CLOUD_DATABASE_NOT_INITIALIZED);
                return false;
            }
            
            if (authManager.IsInitialized == false)
            {
                Log.Warning(Constants.USER_IS_NOT_AUTHENTICATE);
                return false;
            }

            _state = EState.Initializing;
            _local = localDatabase;
            _cloud = cloudDatabase;
            _auth = authManager;
            _state = EState.Ready;
            Log.Info(Constants.INITIALIZED);
            
            return true;
        }

        /// <summary>
        /// 로컬과 클라우드 데이터베이스를 동기화한다.
        /// </summary>
        /// <remarks>더 최신인 데이터를 원본으로 사용한다.</remarks>
        public async Task<bool> SyncAsync()
        {
            if (IsInitialized == false)
            {
                Log.Warning("동기화 실패 (초기화가 안되어 있음)");
                return false;
            }

            if (_auth!.IsAuthenticated == false)
            {
                Log.Warning("동기화 실패 (인증이 안되어 있음)");
                return false;
            }
            
            Log.Info(Constants.SYNCING);
            _state = EState.Syncing;
            
            // 로컬 저장 날짜를 얻는다.
            DateTime localDateTime = _local.LastModifiedDateTime;
            Log.Info($"로컬 데이터베이스 마지막 수정 날짜: {localDateTime} (UTC)");
            Log.Info($"로컬 데이터베이스 마지막 수정 날짜: {localDateTime.ToLocalTime()} (LocalTime)");
            
            // 클라우드 저장 날짜를 얻는다.
            DateTime? cloudDateTime = await _cloud!.GetLastModifiedDateTimeAsync(_auth.UserID);
            if (cloudDateTime == null) cloudDateTime = DateTime.MinValue;
            Log.Info($"클라우드 데이터베이스 마지막 수정 날짜: {cloudDateTime.Value} (UTC)");
            Log.Info($"클라우드 데이터베이스 마지막 수정 날짜: {cloudDateTime.Value.ToLocalTime()} (LocalTime)");
            
            // 어떤게 더 최신 데이터인지 파악한 뒤 싱크한다.
            // 로컬 데이터가 더 최신인 경우:
            if (localDateTime > cloudDateTime.Value)
            {
                Log.Info($"로컬 데이터가 더 최신입니다");
                bool result = await SyncLocalToCloudAsync();
                if (result)
                {
                    Log.Important(Constants.SYNC_COMPLETED);
                }
                else
                {
                    Log.Warning(Constants.SYNC_FAILED);
                    _state = EState.Ready;
                    return false;
                }                
            }
            // 클라우드 데이터가 더 최신인 경우:
            else if (localDateTime < cloudDateTime.Value)
            {
                Log.Info($"클라우드 데이터가 더 최신입니다");
                bool result = await SyncCloudToLocalAsync();
                if (result)
                {
                    Log.Important(Constants.SYNC_COMPLETED);
                }
                else
                {
                    Log.Warning(Constants.SYNC_FAILED);
                    _state = EState.Ready;
                    return false;
                }
            }
            // 로컬과 클라우드가 동일한 경우:
            else
            {
                Log.Info($"로컬과 클라우드 데이터가 같습니다");
                Log.Important(Constants.SYNC_SKIPPED);
            }

            _state = EState.Ready;
            return true;
        }

        /// <summary>
        /// 로컬 데이터를 클라우드로 동기화한다.
        /// </summary>
        public async Task<bool> SyncLocalToCloudAsync()
        {
            if (_local == null || _cloud == null || _auth == null) return false;
            if (_auth.UserID == null) return false;
            
            Log.Info($"로컬 데이터를 클라우드로 올립니다");
            
            // 압축된 세이브 데이터를 준비한다.
            byte[]? bytes = _local.GetArchive();
            if (bytes == null) return false;
            
            // Firestore:
            //   users/{user_id}/lastModifiedTimestamp (Firestore.Timestamp)
            //   users/{user_id}/saveData (BLOB)
            // LocalDatabase:
            //   @System
            //     lastModifiedDateTime (DateTime)
            //
            // 로컬 데이터베이스의 수정 날짜는 바이너리 세이브 데이터 안의 @System.lastModifiedDateTime에 기록된다.
            // 세이브 데이터는 압축되어 바이너리로 데이터베이스에 보관되기 때문에 수정된 날짜를 쉽게 얻을 수 없다.
            // 성능과 편의를 위해 동일한 수정날짜 값을 users/{user_id}/lastModifiedTimestamp에 기록한다.
            //
            // 로컬 데이터베이스의 수정 시간으로 데이터베이스에 기록 할 Timestamp 값을 얻는다.
            DateTime localDateTime;
            Timestamp localTimestamp;
            try
            {
                localDateTime = _local.LastModifiedDateTime.ToUniversalTime();
                localTimestamp = Timestamp.FromDateTime(localDateTime);
            }
            catch (Exception e)
            {
                Log.Warning("로컬 데이터 저장 날짜를 Timestamp로 변환하는데 실패 (예외 발생)");
                Log.Exception(e);
                return false;
            }

            // 클라우드로 올릴 데이터를 준비한다.
            Dictionary<string, object> uploadDict = new Dictionary<string, object>
            {
                [Constants.SAVE_DATA_KEY] = bytes,
                [Constants.LAST_MODIFIED_TIMESTAMP_KEY] = localTimestamp,
                // [Constants.APP_ID_KEY] = Application.identifier,
                [Constants.APP_VERSION_KEY] = Application.version,
                // [Constants.APP_PLATFORM_KEY] = Application.platform.ToString(),
            };

            // 클라우드에 데이터를 넣는다.
            bool setResult = await _cloud.SetDocumentOverwriteAsync(
                Constants.USERS_COLLECTION_NAME, _auth.UserID, uploadDict);
            if (setResult == false) return false;
            
            return true;
        }

        /// <summary>
        /// 클라우드 데이터를 로컬로 동기화한다.
        /// </summary>
        private async Task<bool> SyncCloudToLocalAsync()
        {
            if (_local == null || _cloud == null || _auth == null) return false;
            if (_auth.UserID == null) return false;
            string userId = _auth.UserID;
            
            Log.Info($"클라우드 데이터를 로컬로 가져옵니다");
            
            // TODO: 필요한가? (클라우드 업데이트 날짜를 얻는다)
            // DateTime? cloudDateTime = await _cloud.GetLastModifiedDateTimeAsync(userId);
            // if (cloudDateTime == null) cloudDateTime = DateTime.MinValue;
            
            // 클라우드에 저장된 문서 데이터를 얻는다.
            Dictionary<string, object>? cloudDict = await _cloud.ReadDocumentAsync(Constants.USERS_COLLECTION_NAME, userId);
            if (cloudDict == null) return false;
            
            // 문서에서 Blob를 얻는다.
            if (cloudDict.TryGetValue(Constants.SAVE_DATA_KEY, out object saveData) == false)
            {
                return false;
            }

            // 읽어온 데이트 정리한다. 
            byte[]? bytes;
            try
            {
                Blob blob = (Blob)saveData;
                bytes = blob.ToBytes();
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            if (bytes == null) return false;
            
            // 정리된 데이터로 로컬 데이터베이스를 로드한다.
            bool loadResult = _local.LoadFromArchive(bytes);
            if (loadResult == false) return false;
            
            // TODO: 필요한가? (로컬 업데이트 날짜를 클라우드와 동일하게 맞춘다.)
            // _local.LastModifiedDateTime = cloudDateTime.Value;
            
            return true;            
        }
    }
}