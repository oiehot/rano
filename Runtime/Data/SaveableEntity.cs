// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano.SaveSystem
{
    [DisallowMultipleComponent]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id = string.Empty;
        public string Id => _id;

#if UNITY_EDITOR
        void Reset()
        {
            GenerateId();
        }

        [ContextMenu("Print JsonString", false, 1100)]
        private void PrintJsonString()
        {
            Log.Info($"{_id} States:");
            Debug.Log(ToJsonString());
        }

        //[ContextMenu("Generate Id", false, 1200)]
        private void GenerateId()
        {
            _id = System.Guid.NewGuid().ToString();
        }

#endif

        public object CaptureComponentStates()
        {
            var componentStates = new Dictionary<string, object>();
            foreach (var component in GetComponents<ISaveable>())
            {
                // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
                componentStates[component.GetType().ToString()] = component.CaptureState();
            }
            return componentStates;
        }

        public void RestoreComponentStates(object state)
        {
            var componentStates = (Dictionary<string, object>)state;
            foreach (var component in GetComponents<ISaveable>())
            {
                // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
                string componentName = component.GetType().ToString();
                if (componentStates.TryGetValue(componentName, out object componentState))
                {
                    component.RestoreState(componentState);
                }
            }
        }

        public void Save()
        {
            object state = CaptureComponentStates();
            string jsonString = Rano.Encoding.Json.ConvertObjectToString(state);
            AppStorage.SetString(_id, jsonString);
        }

        public void Load()
        {
            string jsonString = AppStorage.GetString(_id);
            object state = Rano.Encoding.Json.ConvertStringToObject<Dictionary<string, object>>(jsonString)
            RestoreComponentStates(state);
        }

        public string ToJsonString()
        {
            var states = CaptureComponentStates();
            return Rano.Encoding.Json.ConvertObjectToString(states);
        }
    }
}