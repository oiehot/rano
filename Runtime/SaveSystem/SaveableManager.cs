#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    public sealed class SaveableManager : ManagerComponent
    {
        public IDatabase? Database { get; set; }
        public bool IncludeInactive { get; set; } = true;
        public bool AutoSaveOnPause { get; set; } = false;
        public bool AutoSaveOnFocusOut { get; set; } = false;
        public bool AutoSaveOnQuit { get; set; } = true;
        public Action OnBeforeSave { get; set; } = default!;

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (AutoSaveOnQuit == true)
            {
                Log.Info("OnApplicationQuit");
                Log.Info("AutoSaveOnQuit가 켜져 있으므로 모든 Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAll();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true && AutoSaveOnPause)
            {
                Log.Info($"OnApplicationPause({pause})");
                Log.Info("AutoSaveOnPause가 켜져 있으므로 모든 Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAll();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false && AutoSaveOnFocusOut)
            {
                Log.Info($"OnApplicationFocus({focus})");
                Log.Info("OnApplicationFocus가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAll();
            }
        }

        /// <summary>
        /// 게임오브젝트를 저장한다.
        /// </summary>
        /// <param name="go">SaveableEntity 컴포넌트가 부착된 게임오브젝트</param>
        /// <returns>저장 결과</returns>
        public bool Save(GameObject go)
        {
            SaveableEntity saveableEntity;
            
            if (go.TryGetComponent<SaveableEntity>(out saveableEntity) == true)
            {
                return this.Save(saveableEntity);
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
        public bool Save(SaveableEntity saveableEntity)
        {
            if (Database == null)
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
                    Database.SetDictionary(saveableEntity.Id, states);
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
        [ContextMenu("Save All")]
        public bool SaveAll()
        {
            // TODO: 저장전 백업 혹은 임시 슬롯에 저장하고 메인에 덮어쓰기 하기.
            // TODO: SaveableEntity.Order 순서대로 정리한 뒤 차례대로 저장하도록 할것.
            // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
            
            // 저장전에 이벤트를 발생시켜 필요한 데이터를 데이터베이스에 즉시 올릴 수 있는 기회를 제공한다. 
            OnBeforeSave?.Invoke();
            
            SaveableEntity[] saveableEntities = GameObject.FindObjectsOfType<SaveableEntity>(IncludeInactive);
            
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                // 하나의 SaveableEntity 저장에 실패하면 전체 실패처리.
                if (this.Save(saveableEntity) == false)
                {
                    return false;
                }
            }
            
            // 데이터베이스를 저장한다.
            try
            {
                Database?.Save();
            }
            catch (Exception e)
            {
                Log.Warning($"데이터베이스 저장중 예외발생");
                Log.Exception(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 게임오브젝트의 상태를 로드한다.
        /// </summary>
        /// <param name="ㅎo">로드 할 게임오브젝트 (SaveableEntity 컴포넌트가 부착된)</param>
        /// <param name="useDefaultStateIfFailed">로드 실패시 기본상태 사용여부</param>
        /// <returns>로드 결과</returns>
        public bool Load(GameObject go, bool useDefaultStateIfFailed)
        {
            SaveableEntity saveableEntity;
            
            if (go.TryGetComponent<SaveableEntity>(out saveableEntity) == true)
            {
                return this.Load(saveableEntity, useDefaultStateIfFailed);
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
        public bool Load(SaveableEntity saveableEntity, bool useDefaultStateIfFailed)
        {
            if (Database == null)
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
                states = Database.GetDictionary(saveableEntity.Id);
            }
            catch (Exception e)
            {
                Log.Warning($"데이터베이스에서 상태를 얻는중 예외가 발생 (id:{saveableEntity.Id})");
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
                    Log.Warning($"저장된 데이터가 없어 로드하지 못했습니다 (id:{saveableEntity.Id})");
                    return false;                    
                }
            }
        }

        [ContextMenu("Load All")]
        public void LoadAll()
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
            Log.Info($"  IncludeInactive: {IncludeInactive}");
            Log.Info($"  AutoSaveOnPause: {AutoSaveOnPause}");
            Log.Info($"  AutoSaveOnFocusOut: {AutoSaveOnFocusOut}");
            Log.Info($"  AutoSaveOnExit: {AutoSaveOnQuit}");
        }
    }
}