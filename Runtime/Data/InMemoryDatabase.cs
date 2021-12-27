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
        public string LastModifiedDateField => "$InMemoryDatabase.LastModifiedDate";
        public string SavePath { get; private set; }
        public string TemporarySavePath { get; private set; }
        private Dictionary<string, object> _dict;

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
            Load();
        }

        private void UpdateLastModifiedDate()
        {
            _dict[LastModifiedDateField] = DateTime.Now.ToString();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Log.Info("애플리케이션 포커스 일시정지 => 저장.");
                UpdateAllSaveEntities();
                Save();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false)
            {
                Log.Info("애플리케이션 포커스 아웃 => 저장.");
                UpdateAllSaveEntities();
                Save();
            }
        }

        /// <summary>
        /// 모바일앱에서 앱 종료시 이 함수의 실행을 보증하지 않음.
        /// </summary>
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            UpdateAllSaveEntities();
            Save();
        }

        private void UpdateAllSaveEntities()
        {
            // TODO: IncludeInactive 설정화
            bool IncludeInactive = true;
            foreach (var saveable in FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");
                _dict[id] = saveable.CaptureToDict();
            }
        }

        public bool HasKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        //public void Set(string key, object value)
        //{
        //    Dict[key] = value;
        //}

        public void SetDictionary(string key, Dictionary<string, object> value)
        {
            _dict[key] = value;
            UpdateLastModifiedDate();
        }

        public Dictionary<string, object> GetDictionary(string key)
        {
            return (Dictionary<string,object>)_dict[key];
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

        // TODO: 압축 및 암호화 필요.
        /// <summary>
        /// 메모리DB를 로컬파일에 저장한다.
        /// </summary>
        public void Save()
        {
            Log.Info($"{TemporarySavePath} 저장중...");
            try
            {
                using (var fileStream = File.Open(TemporarySavePath, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, _dict);
                }
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
        public bool Load()
        {
            Dictionary<string, object> dict;

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
                using (FileStream fileStream = File.Open(SavePath, FileMode.Open))
                {
                    var binaryFormatter = new BinaryFormatter();
                    dict = (Dictionary<string, object>)binaryFormatter.Deserialize(fileStream);
                }
            }
            catch
            {
                Log.Warning($"{SavePath} 읽기 실패.");
                return false;
            }

            _dict.Clear();
            _dict = dict;
            Log.Info($"{SavePath} 읽기완료.");
            return true;
        }

        [ContextMenu("Clear Data", false, 1001)]
        public void Clear()
        {
            _dict.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Print Json", false, 1002)]
        private void PrintJsonString()
        {
            string str = Rano.Encoding.Json.ConvertObjectToString(_dict);
            Debug.Log(str);
        }
#endif
    }
}
