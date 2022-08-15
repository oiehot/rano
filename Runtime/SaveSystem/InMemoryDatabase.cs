#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano.IO;

namespace Rano.SaveSystem
{
    public sealed class InMemoryDatabase : IDatabase
    {
        private Dictionary<string, object> _dict;
        public string LastModifiedDateKey => $"{typeof(InMemoryDatabase).ToString()}.LastModifiedDate";
        public string SavePath { get; private set; }
        public string TemporarySavePath { get; private set; }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private PrefsBool _resetOnStartPref =
            new PrefsBool($"{typeof(InMemoryDatabase).ToString()}.ResetOnStart");        
        public string JsonSavePath => $"{SavePath}.json";
        public string TemporaryJsonSavePath => $"{JsonSavePath}.tmp";
        public bool UseResetOnStart
        {
            get
            {
                return _resetOnStartPref.Value;
            }
            set
            {
                string status = value ? "Enable" : "Disable";
                Log.Info($"{status} {nameof(UseResetOnStart)}");
                _resetOnStartPref.Value = value;
            }
        }
#endif

        public InMemoryDatabase()
        {
            Log.Sys($"{typeof(InMemoryDatabase).ToString()}: Construction", caller: false);

            _dict = new Dictionary<string, object>();
            SavePath = $"{Application.persistentDataPath}/save";
            TemporarySavePath = $"{SavePath}.tmp";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (UseResetOnStart)
            {
                Log.Warning($"ResetOnStart 플래그가 켜져있어 로드하지 않고 시작합니다.");
            }
            else
            {
                Load();
            }
#else
            Load();
#endif
        }
        
        /// <summary>
        /// 로컬파일을 읽어 메모리DB로 로드한다.
        /// </summary>
        public bool Load()
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
                var bytes = Rano.IO.LocalFile.ReadBytes(SavePath);
                object data = Rano.Encoding.Binary.ConvertBinaryToObject(bytes);
                loadedDict = (Dictionary<string, object>)data;
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
        
        /// <summary>
        /// 메모리DB를 로컬파일에 저장한다.
        /// </summary>
        public void Save()
        {
            Log.Info($"{TemporarySavePath} 저장중...");
            Log.Todo("압축 및 암호화 필요.");
            try
            {
                var bytes = Rano.Encoding.Binary.ConvertObjectToBinary(_dict);
                Rano.IO.LocalFile.WriteBytes(TemporarySavePath, bytes);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // 개발판에서는 json 파일도 저장한다.
                Log.Info($"{TemporaryJsonSavePath} 저장중...");
                Log.Todo("파일스트림을 이용해서 저장해야함.");
                var jsonString = Rano.Encoding.Json.ConvertObjectToString(_dict);
                Rano.IO.LocalFile.WriteString(TemporaryJsonSavePath, jsonString);
#endif

            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                Log.Warning($"{TemporarySavePath} 저장실패");
                return;
            }
            // 임시로 저장한 파일을 정식파일로 승격.
            Rano.IO.LocalFile.Move(TemporarySavePath, SavePath, true);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Rano.IO.LocalFile.Move(TemporaryJsonSavePath, JsonSavePath, true);
#endif

            Log.Info($"{SavePath} 저장완료.");
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public string? GetString(string key, string? defaultValue = null)
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
        
        public void SetString(string key, string value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }

        public bool GetBool(string key)
        {
            if (_dict.TryGetValue(key, out object value))
            {
                return (bool)value;
            }
            else
            {
                throw new NotFoundDataException($"데이터를 찾을 수 없음 (key: {key})");
            }
        }

        public void SetBool(string key, bool value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }

        public Dictionary<string, object>? GetDictionary(string key)
        {
            if (_dict.TryGetValue(key, out object value))
            {
                return (Dictionary<string, object>)value;
            }
            else
            {
                return null;
            }
        }
        
        public void SetDictionary(string key, Dictionary<string, object> value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }        

        public bool HasKey(string key)
        {
            return _dict.ContainsKey(key);
        }
        
        private void UpdateLastModifiedDate()
        {
            _dict[LastModifiedDateKey] = DateTime.Now.ToString();
        }

        public void LogStatus()
        {
            Log.Info($"{nameof(InMemoryDatabase)}");
            Log.Info($"  LastModifiedDate Key: {LastModifiedDateKey}");
            Log.Info($"  LastModifiedDate: {_dict[LastModifiedDateKey]}");
            Log.Info($"  SavePath: {SavePath}");
            Log.Info($"  TemporarySavePath: {TemporarySavePath}");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log.Info($"  JsonSavePath: {JsonSavePath}");
            Log.Info($"  TemporaryJsonSavePath: {TemporaryJsonSavePath}");
            Log.Info($"  ResetOnStart Key: {_resetOnStartPref.Key}");
            Log.Info($"  {nameof(UseResetOnStart)}: {UseResetOnStart}");
#endif
        }
    }
}
