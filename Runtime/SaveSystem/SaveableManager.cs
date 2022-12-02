#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano.Database;

namespace Rano.SaveSystem
{
    /// <summary>
    /// 게임 오브젝트들의 정보를 모아 로컬 데이터베이스에 저장해준다.
    /// </summary>
    public sealed class SaveableManager : ManagerComponent
    {
        private ILocalDatabase? _localDatabase;
        private bool _includeInactive = true;
        
        public ILocalDatabase? LocalDatabase => _localDatabase;
        public bool IsInitialized => _localDatabase != null;

        /// <summary>
        /// 초기화한다.
        /// </summary>
        /// <param name="localDatabase">게임 데이터들을 저장할 로컬 데이터베이스</param>
        public void Initialize(ILocalDatabase localDatabase)
        {
            _localDatabase = localDatabase;
        }

        /// <summary>
        /// 게임오브젝트를 저장한다.
        /// </summary>
        /// <param name="go">SaveableEntity 컴포넌트가 부착된 게임오브젝트</param>
        /// <returns>저장 결과</returns>
        public bool SaveToDatabase(GameObject go)
        {
            SaveableEntity saveableEntity;
            
            if (go.TryGetComponent<SaveableEntity>(out saveableEntity))
            {
                return this.SaveToDatabase(saveableEntity);
            }
            else
            {
                Log.Warning($"게임오브젝트에 {nameof(SaveableEntity)} 컴포넌트가 없어 저장하지 못했습니다 ({go.name})");
                return false;
            }
        }

        /// <summary>
        /// SaveableEntity로 게임오브젝트를 저장한다.
        /// </summary>
        /// <param name="saveableEntity">게임오브젝트에 부착된 SaveableEntity 컴포넌트</param>
        /// <returns>저장 결과</returns>
        public bool SaveToDatabase(SaveableEntity saveableEntity)
        {
            if (_localDatabase == null)
            {
                Log.Warning($"데이터베이스가 지정되어있지 않아 저장하지 못했습니다");
                return false;
            }

            if (String.IsNullOrEmpty(saveableEntity.Id))
            {
                Log.Warning($"{nameof(SaveableEntity)}의 이름이 비어있어 저장하지 못했습니다");
                return false;
            }

            Log.Info($"게임오브젝트 저장중 ({saveableEntity.Id})");
            var states = saveableEntity.CaptureState();
            if (states != null)
            {
                try
                {
                    _localDatabase.SetDictionary(saveableEntity.Id, states);
                }
                catch (Exception e)
                {
                    Log.Warning($"데이터베이스에 상태를 저장하는중 예외가 발생 (id:{saveableEntity.Id})");
                    Log.Exception(e);
                    return false;
                }
            }
            else
            {
                // 캡쳐한 데이터가 없음
                return false;
            }
            return true;
        }

        /// <summary>
        /// 모든 게임오브젝트들을 저장한다. (SaveableEntity 컴포넌트가 부착된)
        /// </summary>
        /// <returns>저장 결과, 하나의 게임오브젝트라도 저장에 실패하면 false</returns>
        [ContextMenu(nameof(SaveAllToDatabase))]
        public bool SaveAllToDatabase()
        {
            // TODO: 저장전 백업 혹은 임시 슬롯에 저장하고 메인에 덮어쓰기 하기.
            // TODO: SaveableEntity.Order 순서대로 정리한 뒤 차례대로 저장하도록 할것.
            // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
            SaveableEntity[] saveableEntities = GameObject.FindObjectsOfType<SaveableEntity>(_includeInactive);
            
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                // 하나의 SaveableEntity 저장에 실패하면 전체 실패처리.
                if (this.SaveToDatabase(saveableEntity) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 게임오브젝트의 상태를 로드한다.
        /// </summary>
        /// <param name="go">로드 할 게임오브젝트 (SaveableEntity 컴포넌트가 부착된)</param>
        /// <param name="useDefaultStateIfFailed">로드 실패시 기본상태 사용여부</param>
        /// <returns>로드 결과</returns>
        public bool LoadFromDatabase(GameObject go, bool useDefaultStateIfFailed)
        {
            SaveableEntity saveableEntity;
            
            if (go.TryGetComponent<SaveableEntity>(out saveableEntity))
            {
                return this.LoadFromDatabase(saveableEntity, useDefaultStateIfFailed);
            }
            else
            {
                Log.Warning($"게임오브젝트에 {nameof(SaveableEntity)} 컴포넌트가 없어 로드하지 못했습니다 ({go.name})");
                return false;
            }
        }
        
        /// <summary>
        /// SaveableEntity로 게임오브젝트의 상태를 로드한다.
        /// </summary>
        /// <param name="saveableEntity">로드 할 게임오브젝트에 부착된 SaveableEntity컴포넌트</param>
        /// <param name="useDefaultStateIfFailed">로드 실패시 기본상태 사용여부</param>
        /// <returns>로드 결과</returns>
        public bool LoadFromDatabase(SaveableEntity saveableEntity, bool useDefaultStateIfFailed)
        {
            if (_localDatabase == null)
            {
                Log.Warning($"데이터베이스가 연결되어있지 않아 로드하지 못했습니다");
                return false;
            }
            
            if (String.IsNullOrEmpty(saveableEntity.Id))
            {
                Log.Warning($"{nameof(SaveableEntity)}의 Id가 비어있어 로드하지 못했습니다");
                return false;
            }            
            
            Dictionary<string, object>? states;
            try
            {
                states = _localDatabase.GetDictionary(saveableEntity.Id);
            }
            catch (Exception e)
            {
                Log.Info($"데이터베이스에서 상태를 얻는중 예외가 발생했습니다 (id:{saveableEntity.Id})");
                Log.Exception(e);
                states = null;
            }
            
            if (states != null)
            {
                return saveableEntity.RestoreState(states, useDefaultStateIfFailed);
            }
            else
            {
                if (useDefaultStateIfFailed)
                {
                    return saveableEntity.DefaultState();
                }
                else
                {
                    Log.Info($"저장된 데이터가 없어 로드하지 않았습니다 (id:{saveableEntity.Id})");
                    return false;                    
                }
            }
        }

        [ContextMenu(nameof(LoadAllFromDatabase))]
        public void LoadAllFromDatabase()
        {
            throw new NotImplementedException();
            // TODO: 1) 모든 SaveableEntity를 모은다.
            // TODO: 2) Order 순으로 정렬한다 (오른차순)
            // TODO: 3) 정렬한 순서대로 데이터베이스에서 상태를 가져오고 Restore 한다.
        }

        [ContextMenu(nameof(LogStatus))]
        public void LogStatus()
        {
            Log.Info($"{nameof(SaveableManager)}");
            Log.Info($"  IncludeInactive: {_includeInactive}");
        }
    }
}