// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using Rano.IO;

namespace Rano.SaveSystem
{
    public sealed class InMemoryDatabase : Singleton<InMemoryDatabase>
    {
        private Dictionary<string, object> _dict;
        public string LastModifiedDateField => $"{typeof(InMemoryDatabase).ToString()}.LastModifiedDate";
        public string SavePath { get; private set; }
        public string TemporarySavePath { get; private set; }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public string JsonSavePath => $"{SavePath}.json";
        public string TemporaryJsonSavePath => $"{JsonSavePath}.tmp";
        public PrefsBool ResetOnStart { get; private set; } =
            new PrefsBool($"{typeof(InMemoryDatabase).ToString()}.ResetOnStart");
#endif

        public InMemoryDatabase()
        {
            Log.Sys($"{typeof(InMemoryDatabase).ToString()}: Construction", caller: false);

            _dict = new Dictionary<string, object>();
            SavePath = $"{Application.persistentDataPath}/save";
            TemporarySavePath = $"{SavePath}.tmp";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (ResetOnStart.Value == true)
            {
                Log.Warning($"{ResetOnStart.Key}가 켜져있어 로드하지 않고 시작합니다.");
            }
            else
            {
                Load();
            }
#else
            Load();
#endif
        }

        private void UpdateLastModifiedDate()
        {
            _dict[LastModifiedDateField] = DateTime.Now.ToString();
        }

        private void Clear()
        {
            _dict.Clear();
        }

        /// <summary>
        /// 메모리DB를 로컬파일에 저장한다.
        /// TODO: 압축 및 암호화 필요.
        /// </summary>
        public void Save()
        {
            Log.Info($"{TemporarySavePath} 저장중...");
            try
            {
                var bytes = Rano.Encoding.Binary.ConvertObjectToBinary(_dict);
                Rano.IO.LocalFile.WriteBytes(TemporarySavePath, bytes);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // 개발판에서는 json 파일도 저장한다.
                Log.Info($"{TemporaryJsonSavePath} 저장중...");
                var jsonString = Rano.Encoding.Json.ConvertObjectToString(_dict); // TODO: Save using FileStream
                Rano.IO.LocalFile.WriteString(TemporaryJsonSavePath, jsonString);
#endif

            }
            catch (Exception e)
            {
                Debug.Log(e);
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

        public bool HasKey(string key)
        {
            return _dict.ContainsKey(key);
        }
    }
}
