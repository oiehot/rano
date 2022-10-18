#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rano.Auth.Firebase;
using Rano.Database.FirebaseFirestore;
using Firebase.Firestore;

namespace Rano.Leaderboard
{
    /// <summary>
    /// 리더보드를 관리한다.
    /// </summary>
    public sealed class LeaderboardManager : ManagerComponent
    {
        private const string LEADERBOARDS_COLLECTION_NAME = "leaderboards";
        private const string USER_DISPLAY_NAME_KEY = "userDisplayName";
        private const int MAX_QUERY_COUNT = 100;
        private FirebaseAuthManager? _auth;
        private FirebaseFirestoreManager? _db;
        public bool IsInitialized => (_auth != null && _db != null && _auth.IsInitialized && _db.IsInitialized);

        private static string GetTimestampKeyName(string leaderboardName)
        {
            return $"{leaderboardName}Timestamp";
        }
        
        private static bool IsValidKey(string key)
        {
            if (string.IsNullOrEmpty(key) == true) return false;
            return true;
        }

        private static bool IsValidCount(int count)
        {
            return count is > 0 and <= MAX_QUERY_COUNT;
        }
        
        /// <summary>
        /// 리더보드 매니져를 초기화 한다.
        /// </summary>
        public bool Initialize(FirebaseAuthManager authManager, FirebaseFirestoreManager firestoreManager)
        {
            Log.Info("리더보드 초기화중...");

            if (authManager == null)
            {
                Log.Warning("리더보드 초기화 실패 (인증 관리자가 비어 있음)");
                return false;
            }

            if (firestoreManager == null)
            {
                Log.Warning("리더보드 초기화 실패 (데이터베이스 관리자가 비어 있음)");
                return false;
            }
            
            if (authManager.IsInitialized == false)
            {
                Log.Warning("리더보드 초기화 실패 (인증 관리자가 초기화 되지 않음)");
                return false;
            }

            if (firestoreManager.IsInitialized == false)
            {
                Log.Warning("리더보드 초기화 실패 (데이터베이스 관리자가 초기화 되지 않음)");
                return false;
            }
            
            _auth = authManager;
            _db = firestoreManager;
            
            Log.Info("리더보드 초기화 성공");
            
            return true;
        }

        /// <summary>
        /// 리더보드의 값을 설정한다.
        /// </summary>
        public async Task<bool> SetLeaderboardLongAsync(string leaderboardName, long value)
        {
            if (IsInitialized == false)
            {
                Log.Warning($"리더보드 설정 실패 (초기화 되지 않았음)");
                return false;
            }
            if (_auth!.IsAuthenticated == false)
            {
                Log.Warning($"리더보드 설정 실패 (인증 되지 않았음)");
                return false;
            }
            if (IsValidKey(leaderboardName) == false)
            {
                Log.Warning($"리더보드 설정 실패 (올바르지 않은 리더보드 이름: {leaderboardName})");
                return false;
            }

            // 업데이트 할 데이터를 준비한다.
            Dictionary<string,object> dict= new Dictionary<string,object>();
            dict[USER_DISPLAY_NAME_KEY] = _auth!.UserDisplayName;
            dict[leaderboardName] = value;
            dict[GetTimestampKeyName(leaderboardName)] = DateTime.UtcNow;
            
            // 문서 업데이트
            bool result = await _db!.SetDocumentMergeAsync(LEADERBOARDS_COLLECTION_NAME, _auth!.UserId!, dict);
            
            return result;
        }

        /// <summary>
        /// 리더보드를 얻는다.
        /// </summary>
        public async Task<bool> GetLeaderboardAsync(string leaderboardName, int count)
        {
            if (IsInitialized == false)
            {
                Log.Warning($"리더보드를 얻을 수 없음 (초기화 되지 않았음)");
                return false;
            }
            if (_auth!.IsAuthenticated == false)
            {
                Log.Warning($"리더보드를 얻을 수 없음 (인증 되지 않았음)");
                return false;
            }
            if (IsValidKey(leaderboardName) == false)
            {
                Log.Warning($"리더보드를 얻을 수 없음 (올바르지 않은 리더보드 이름: {leaderboardName})");
                return false;
            }

            if (IsValidCount(count) == false)
            {
                Log.Warning($"리더보드를 얻을 수 없음 (올바르지 않은 쿼리 카운트: {count})");
                return false;
            }

            // 리더보드 컬렉션을 얻는다.
            CollectionReference? collectionRef = _db!.GetCollectionReference(LEADERBOARDS_COLLECTION_NAME);
            if (collectionRef == null)
            {
                Log.Warning("리더보드를 얻을 수 없음 (데이터베이스 컬렉션 레퍼런스가 없음)");
                return false;
            }
            
            // 쿼리를 얻는다.
            Query query;
            try
            {
                query = collectionRef.OrderByDescending(leaderboardName).Limit(count);
            }
            catch (Exception e)
            {
                Log.Warning("리더보드를 얻을 수 없음 (데이터베이스 쿼리 중 예외가 발생함)");
                Log.Exception(e);
                return false;
            }
            if (query == null)
            {
                Log.Warning("리더보드를 얻을 수 없음 (데이터베이스 쿼리 결과가 비어있음)");
                return false;
            }

            // 쿼리로 부터 스냅샷을 얻는다
            QuerySnapshot querySnapshot;
            try
            {
                querySnapshot = await query.GetSnapshotAsync();
            }
            catch (Exception e)
            {
                Log.Warning("리더보드를 얻을 수 없음 (쿼리로 부터 스냅샷을 얻는 중 예외가 발생함)");
                Log.Exception(e);
                return false;
            }
            if (querySnapshot == null)
            {
                Log.Warning("리더보드를 얻을 수 없음 (스냅샷이 비어있음)");
                return false;
            }

            if (querySnapshot.Count <= 0)
            {
                Log.Warning("리더보드 내용이 없음");
                return false;
            }

            // 스냅샷으로 부터 문서를 읽는다.
            try
            {
                // TODO: 쿼리 스냅샷 문서를 리더보드 데이터 타입으로 변환하여 리턴할것.
                foreach (DocumentSnapshot document in querySnapshot.Documents)
                {
                    Log.Info($"Document (Id: {document.Id})");
                    Dictionary<string, object> dict = document.ToDictionary();
                    foreach (KeyValuePair<string, object> item in dict)
                    {
                        Log.Info($"  - {item.Key}: {item.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning("리더보드를 얻을 수 없음 (스냅샷으로 부터 문서를 읽는 중 예외가 발생)");
                Log.Exception(e);
                return false;
            }

            return true;
        }
    }
}