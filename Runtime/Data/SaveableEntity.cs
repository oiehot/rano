// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Rano.SaveSystem
{
    [DisallowMultipleComponent]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private bool _autoLoadOnAwake = true;
        [SerializeField] private bool _autoSaveOnDestroy = false;
        public string Id => _id;

#if UNITY_EDITOR
        void Reset()
        {
            GenerateId();
        }

        // 실수로 Id를 바꿀 수 있기 때문에 해제함.
        //[ContextMenu("Generate Id", false, 1200)]
        private void GenerateId()
        {
            _id = System.Guid.NewGuid().ToString();
        }

        [ContextMenu("Print Json", false, 1200)]
        private void PrintJsonString()
        {
            Log.Info($"{_id}:");
            Debug.Log(ToJsonString());
        }

        private string ToJsonString()
        {
            var dict = CaptureToDict();
            return Rano.Encoding.Json.ConvertObjectToString(dict);
        }

#endif

        public Dictionary<string,object> CaptureToDict()
        {
            var dict = new Dictionary<string,object>();
            foreach (var component in GetComponents<ISaveLoadable>())
            {
                // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
                dict[component.GetType().ToString()] = component.CaptureState();
            }
            return dict;
        }

        public byte[] CaptureToBinary()
        {
            byte[] bytes = null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, CaptureToDict());
                bytes = ms.ToArray();
            }
            return bytes;
        }

        public void RestoreFromDict(object dict)
        {
            var componentStates = (Dictionary<string, object>)dict;
            foreach (var component in GetComponents<ISaveLoadable>())
            {
                // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
                string componentName = component.GetType().ToString();
                if (componentStates.TryGetValue(componentName, out object componentState))
                {
                    component.RestoreState(componentState);
                }
            }
        }

        public void RestoreFromBinary(byte[] bytes)
        {
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = bf.Deserialize(ms);
                RestoreFromDict(obj);
            }
        }

        // TODO: AutoLoad
        [ContextMenu("Load", false, 1101)]
        private void Load()
        {
            if (InMemoryDatabase.Instance.HasKey(_id) == true)
            {
                Log.Info($"상태복구 {_id} (InMemoryDatabase)");
                RestoreFromDict(InMemoryDatabase.Instance.GetDictionary(_id));
            }
            else Log.Info($"상태복구생략 {_id} (NotInMemoryDatabase)");
        }

        [ContextMenu("Save", false, 1102)]
        private void Save()
        {
            Log.Info($"상태저장 {_id} (InMemoryDatabase)");
            var dict = CaptureToDict();
            InMemoryDatabase.Instance.SetDictionary(_id, dict);
        }

        private void Awake()
        {
            //Log.Info("Awake");
            if (_autoLoadOnAwake == true) Load();
        }

        private void OnDestroy()
        {
            //Log.Info("OnDestroy");
            if (_autoSaveOnDestroy && InMemoryDatabase.Instance != null)
            {
                Log.Info("OnDestroy => AutoSave");
                Save();
            }
        }
    }
}