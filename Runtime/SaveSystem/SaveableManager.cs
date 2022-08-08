using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    public sealed class SaveableManager : ManagerComponent
    {
        public IDatabase Database { get; set; }
        public bool IncludeInactive { get; set; } = true;
        public bool AutoSaveOnPause { get; set; } = false;
        public bool AutoSaveOnFocusOut { get; set; } = false;
        public bool AutoSaveOnExit { get; set; } = true;
        public Action OnBeforeSave { get; set; }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (AutoSaveOnExit == true)
            {
                Log.Info("OnApplicationQuit");
                Log.Info("AutoSaveOnExit가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true && AutoSaveOnPause)
            {
                Log.Info($"OnApplicationPause({pause})");
                Log.Info("AutoSaveOnPause가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false && AutoSaveOnFocusOut)
            {
                Log.Info($"OnApplicationFocus({focus})");
                Log.Info("OnApplicationFocus가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }
        
        public void Save()
        {
            // 저장전에 이벤트를 발생시켜 필요한 데이터를 데이터베이스에 즉시 올릴 수 있는 기회를 제공한다. 
            OnBeforeSave?.Invoke();
            
            // Capture All Entities To Database
            var saveableEntities = GameObject.FindObjectsOfType<SaveableEntity>(IncludeInactive);
            foreach (var saveableEntity in saveableEntities)
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveableEntity.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");
                var gameObjectState = saveableEntity.CaptureState(); 
                Database?.SetDictionary(id, gameObjectState);
            }
            
            // 메모리 데이터베이스를 파일시스템에 저장한다.
            Database?.Save();
        }

        public void Load()
        {
            throw new NotImplementedException();
            
            // TODO: 1) 모든 SaveableEntity를 모은다.
            // TODO: 2) Order 순으로 정렬한다 (오른차순)
            // TODO: 3) 정렬한 순서대로 데이터베이스에서 상태를 가져오고 Restore 한다.
        }

        public void SaveStateToDatabase(string id, Dictionary<string, object> state)
        {
            if (Database != null)
            {
                Database.SetDictionary(id, state);
            }
            else
            {
                Log.Warning($"데이터베이스가 지정되어있지 않아 데이터를 저장할 수 없습니다 (key:{id})");
            }
        }

        public Dictionary<string, object> GetStateFromDatabase(string id)
        {
            if (Database != null)
            {
                return Database.GetDictionary(id);
            }
            else
            {
                Log.Warning($"데이터베이스가 지정되어있지 않아 데이터를 얻을 수 없습니다 (key:{id})");
                return null;
            }
        }

        /// <summary>
        /// 데이터베이스에 특정 id키를 가진 데이터가 있는지 여부를 리턴한다.
        /// </summary>
        /// <param name="id">데이터의 Key</param>
        /// <returns>데이터 존재 여부</returns>
        public bool HasData(string id)
        {
            if (Database != null)
            {
                return Database.HasKey(id);
            }

            return false;
        }
        
        [ContextMenu(nameof(LogStatus))]
        public void LogStatus()
        {
            Log.Info($"{nameof(SaveableManager)}");
            Log.Info($"  IncludeInactive: {IncludeInactive}");
            Log.Info($"  AutoSaveOnPause: {AutoSaveOnPause}");
            Log.Info($"  AutoSaveOnFocusOut: {AutoSaveOnFocusOut}");
            Log.Info($"  AutoSaveOnExit: {AutoSaveOnExit}");
        }
    }
}