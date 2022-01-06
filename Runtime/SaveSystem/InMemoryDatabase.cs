// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    public sealed class InMemoryDatabase : MonoSingleton<InMemoryDatabase>
    {
        private Dictionary<string, object> _dict;
        public string LastModifiedDateField => "$InMemoryDatabase.LastModifiedDate";
        public string SavePath { get; private set; }
        public string TemporarySavePath { get; private set; }
        public bool AutoSaveOnExit { get; private set; } = true;
        public bool IncludeInactive { get; private set; } = true;
        public Action OnExit { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _dict = new Dictionary<string, object>();
            SavePath = $"{Application.persistentDataPath}/memory.db";
            TemporarySavePath = $"{SavePath}.tmp";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadFromFile();
        }

        private void UpdateLastModifiedDate()
        {
            _dict[LastModifiedDateField] = DateTime.Now.ToString();
        }

#if !UNITY_EDITOR
        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Log.Info("OnApplicationPause");
                OnApplicationPauseOrQuit();
            }
            else
            {
                Log.Info("OnApplicationResume");
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == true)
            {
                Log.Info("OnApplicationFocusIn");
            }
            else
            {
                Log.Info("OnApplicationFocusOut");
                OnApplicationPauseOrQuit();
            }
        }
#endif

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            Log.Info("OnApplicationQuit");
            OnApplicationPauseOrQuit();
        }

        private void OnApplicationPauseOrQuit()
        {
            Log.Info("OnApplicationPauseOrQuit");
            if (AutoSaveOnExit)
            {
                Log.Info("AutoSaveOnExit가 켜져있어 자동으로 파일에 저장합니다");
                OnExit?.Invoke();
                CaptureAllSaveableEntities();
                SaveToFile();
            }
            else
            {
                Log.Info("AutoSaveOnExit가 켜져있지 않아 파일에 저장하지 않습니다");
            }
        }

        private void CaptureAllSaveableEntities()
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");
                _dict[id] = saveable.CaptureState();
            }
        }

        public bool HasKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public void SetDictionary(string key, Dictionary<string, object> value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }

        public void SetString(string key, string value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }

        public string GetString(string key, string defaultValue = null)
        {
            object value;

            if (_dict.TryGetValue(key, out value))
            {
                return (string)value;
            }
            else
            {
                return defaultValue;
            }
        }

        public Dictionary<string, object> GetDictionary(string key)
        {
            return (Dictionary<string, object>)_dict[key];
        }

        public void Clear()
        {
            _dict.Clear();
        }

        /// <summary>
        /// 메모리DB를 로컬파일에 저장한다.
        /// TODO: 압축 및 암호화 필요.
        /// </summary>
        public void SaveToFile()
        {
            Log.Info($"{TemporarySavePath} 저장중...");
            try
            {
                // TODO: Save using FileStream
                string jsonString = Rano.Encoding.Json.ConvertObjectToString(_dict);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log(jsonString);
#endif
                Rano.IO.LocalFile.WriteString(TemporarySavePath, jsonString);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Log.Warning($"{TemporarySavePath} 저장실패");
                return;
            }
            // 임시로 저장한 파일을 정식파일로 승격.
            Rano.IO.LocalFile.Move(TemporarySavePath, SavePath, true);

            Log.Info($"{SavePath} 저장완료.");
        }

        /// <summary>
        /// 로컬파일을 읽어 메모리DB로 로드한다.
        /// </summary>
        public bool LoadFromFile()
        {
            Dictionary<string, object> loadedDict;

            // 세이브 파일 체크
            if (!System.IO.File.Exists(SavePath))
            {
                Log.Info($"{SavePath} 저장파일이 없어 로드하지 않습니다.");
                return false;
            }

            // 저장된 파일 읽기
            Log.Info($"{SavePath} 읽는중...");
            try
            {
                // TODO: Load using FileStream
                string jsonString = Rano.IO.LocalFile.ReadString(SavePath);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log(jsonString);
#endif
                object result = Rano.Encoding.Json.ConvertStringToObject(jsonString);
                loadedDict = (Dictionary<string, object>)result;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Log.Warning($"{SavePath} 읽기 실패.");
                return false;
            }

            _dict.Clear();
            _dict = loadedDict;
            Log.Info($"{SavePath} 읽기완료.");
            return true;
        }

#if UNITY_EDITOR
        [ContextMenu("Print Json", false, 1302)]
        private void PrintJsonString()
        {
            string jsonString = Rano.Encoding.Json.ConvertObjectToString(_dict);
            Debug.Log(jsonString);
        }
#endif
    }
}
