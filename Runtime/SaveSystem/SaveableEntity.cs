using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    [DisallowMultipleComponent]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id = string.Empty;
        
        /// <summary>
        /// 배치시 로드 순서. 작을수록 더 먼저 로드된다.
        /// </summary>
        [SerializeField] private int _order = 0;
        
        /// <summary>
        /// Awake시 자동으로 로드되는지 여부
        /// </summary>
        [SerializeField] private bool _autoLoad = false;
        
        /// <summary>
        /// Destroy시 자동으로 저장할지 여부
        /// </summary>
        [SerializeField] private bool _autoSaveOnDestroy = false;
        
        public string Id => _id;
        public int Order => _order;

        void Awake()
        {
            if (_autoLoad)
            {
                try
                {
                    Log.Info($"Load from Database ({_id})");
                    RestoreFromDatabase();
                }
                catch (Exception e)
                {
                    Log.Info($"저장된 데이터가 없거나 복구중에 예외가 발생하여 초기값으로 설정합니다. ({_id})");
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                    Debug.LogWarning(e);
#endif
                    DefaultState();
                }
            }
        }

        void OnDestroy()
        {
            if (_autoSaveOnDestroy)
            {
                Log.Info($"Save to Database ({_id})");
                SaveToDatabase();
            }
        }

        void Reset()
        {
            GenerateId();
        }

        // 실수로 Id를 바꿀 수 있기 때문에 해제함.
        //[ContextMenu("Generate Id", false, 1401)]
        private void GenerateId()
        {
            _id = System.Guid.NewGuid().ToString();
        }

        [ContextMenu("Set to ClearState")]
        public void ClearState()
        {
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                saveableComponent.ClearState();
            }
        }

        [ContextMenu("Set to DefaultState")]
        public void DefaultState()
        {
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                saveableComponent.DefaultState();
            }
        }

        public Dictionary<string, object> CaptureState()
        {
            var dict = new Dictionary<string, object>();
            foreach (var component in GetComponents<ISaveLoadable>())
            {
                // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
                dict[component.GetType().ToString()] = component.CaptureState();
            }
            return dict;
        }

        public void RestoreState(object dict)
        {
            var componentStates = (Dictionary<string, object>)dict;
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                // TODO: 두 개 이상의 동일컴포넌트가 있는 예외상황 대처.
                string saveableComponentName = saveableComponent.GetType().ToString();
                if (componentStates.TryGetValue(saveableComponentName, out object componentState) == false)
                {
                    throw new ComponentDataNotFoundException(_id, saveableComponentName);
                }
                saveableComponent.ValidateState(componentState);
                saveableComponent.RestoreState(componentState);
            }
        }

        [ContextMenu("Save To Database")]
        public void SaveToDatabase()
        {
            var gameObjectState = CaptureState();
            SaveableManager saveableManager = GameObject.FindObjectOfType<SaveableManager>(includeInactive:true);
            saveableManager.SaveStateToDatabase(_id, gameObjectState);
        }

        [ContextMenu("Restore From Database")]
        public void RestoreFromDatabase()
        {
            SaveableManager saveableManager = GameObject.FindObjectOfType<SaveableManager>(includeInactive:true);
            if (saveableManager.HasData(_id))
            {
                var stateDict = saveableManager.GetStateFromDatabase(_id);
                RestoreState(stateDict);
            }
            else
            {
                throw new GameObjectDataNotFoundException(_id);
            }
        }

        public string ToJsonString()
        {
            var dict = CaptureState();
            return Rano.Encoding.Json.ConvertObjectToString(dict);
        }

        [ContextMenu("Print Json", false, 1301)]
        public void PrintJsonString()
        {
            Log.Info($"{_id}:");
            Debug.Log(ToJsonString());
        }
    }
}