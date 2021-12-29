// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    [DisallowMultipleComponent]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private bool _autoLoadOnAwake = false;
        [SerializeField] private bool _autoSaveOnDestroy = false;
        public string Id => _id;

        void Awake()
        {
            //Log.Info("Awake");
            if (_autoLoadOnAwake == true) RestoreFromMemory();
        }

        void OnDestroy()
        {
            //Log.Info("OnDestroy");
            if (_autoSaveOnDestroy && InMemoryDatabase.Instance != null)
            {
                Log.Info("OnDestroy => AutoSave");
                CaptureToMemory();
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

        public void SetId(string id)
        {
            // TODO: Validate
            _id = id;
        }

        public void ClearState()
        {
            Log.Info($"빈상태로 설정 ({_id})");
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                saveableComponent.ClearState();
            }
        }

        public void DefaultState()
        {
            Log.Info($"기본상태로 설정 ({_id})");
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
                if (componentStates.TryGetValue(saveableComponentName, out object componentState) == false) continue;

                try
                {
                    saveableComponent.ValidateState(componentState);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    Log.Warning($"저장된 데이터가 정상이 아닙니다. 기본상태로 설정합니다 ({saveableComponentName})");
                    saveableComponent.DefaultState();
                    continue;
                }

                try
                {
                    saveableComponent.RestoreState(componentState);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    Log.Warning($"컴포넌트 상태복구에 실패. 기본상태로 설정합니다 ({saveableComponentName})");
                    saveableComponent.DefaultState();
                }

                // Done
            }
        }

        public void CaptureToMemory()
        {
            Log.Info($"상태저장 {_id} (InMemoryDatabase)");
            var dict = CaptureState();
            InMemoryDatabase.Instance.SetDictionary(_id, dict);
        }

        public void RestoreFromMemory()
        {
            if (InMemoryDatabase.Instance.HasKey(_id) == true)
            {
                Log.Info($"상태복구 {_id} (InMemoryDatabase)");
                RestoreState(InMemoryDatabase.Instance.GetDictionary(_id));
            }
            else
            {
                Log.Info($"저장데이터 없음 {_id}");
                DefaultState();
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

        //public byte[] CaptureToBinary()
        //{
        //    byte[] bytes = null;
        //    bytes = Rano.Encoding.Binary.ConvertObjectToBinary(
        //        CaptureToDict()
        //    );
        //    return bytes;
        //}

        //public void RestoreFromBinary(byte[] bytes)
        //{
        //    object obj = Rano.Encoding.Binary.ConvertBinaryToObject(bytes);
        //    RestoreFromDict(obj);
        //}
    }
}