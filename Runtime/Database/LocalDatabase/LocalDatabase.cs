#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.Database
{
    [Serializable]
    public struct SLocalDatabaseData
    {
        public string appId;
        public string appVersion;
        public string appPlatform;
        public DateTime lastModifiedDateTime;
    }
    
    public sealed partial class LocalDatabase : ILocalDatabase
    {
        private const string SYSTEM_DATA_KEY = "@System";
        
        private enum EState
        {
            None = 0,
            Initializing,
            Ready
        }
        private EState _state;
        private Dictionary<string, object> _dict;
        private readonly string _savePath;
        private DateTime _lastModifiedDateTime;
        
        public bool IsInitialized => _state >= EState.Ready;

        public DateTime LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => _lastModifiedDateTime = value;
        }

        public LocalDatabase()
        {
            _state = EState.None;
            _dict = new Dictionary<string, object>();
            _savePath = $"{Application.persistentDataPath}/save";
        }
        
        public void Initialize()
        {
            if (_state != EState.None)
            {
                Log.Warning($"이미 초기화되어 있습니다");
                return;
            }
            
            Log.Sys($"{typeof(LocalDatabase)}: 초기화 중...", caller: false);
            Log.Info($"  저장 경로: {_savePath}");
            
            _state = EState.Initializing;
            _dict.Clear();
            
#if DISABLE_LOCAL_DB_LOAD
            Log.Important($"로드 생략 (DISABLE_LOCAL_DB_LOAD)");
            _state = EState.Ready;
#else
            if (Load() == false)
            {
                _dict.Clear();
            }
            _state = EState.Ready;
#endif
        }

        private Dictionary<string, object> GetDictionaryArchive()
        {
            // 클래스 자체의 정보는 Dictionary에 저장되지 않으므로
            // 클래스 정보를 취합하여 Dictionary에 넣는다.
            object systemData = CaptureSystemData();
            _dict[SYSTEM_DATA_KEY] = systemData;
            return _dict;
        }
        
        public bool Load()
        {
            Log.Info($"로드 중... ({_savePath})");
            if (LoadFromFile(_savePath))
            {
                Log.Info($"로드된 세이브 파일의 마지막 수정 시간: {_lastModifiedDateTime} (UTC)");
                Log.Info($"로드된 세이브 파일의 마지막 수정 시간: {_lastModifiedDateTime.ToLocalTime()} (LocalTime)");
                Log.Info("로드 성공");
                return true;
            }
            else
            {
                Log.Warning("로그 실패");
                return false;
            }
        }

        public bool Save()
        {
            return SaveAsFile(_savePath);
        }

        public void Clear()
        {
            _dict.Clear();
        }
        
        private object CaptureSystemData()
        {
            var state = new SLocalDatabaseData
            {
                appId = Application.identifier,
                appVersion = Application.version,
                appPlatform = Application.platform.ToString(),
                lastModifiedDateTime = _lastModifiedDateTime
                // lastSavedDateTime = _lastSavedDateTime
            };
            return state;
        }
        
        private void RestoreSystemData(object state)
        {
            SLocalDatabaseData data = (SLocalDatabaseData)state;
            _lastModifiedDateTime = data.lastModifiedDateTime;
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

        public bool TryGetBool(string key, out bool result)
        {
            if (_dict.TryGetValue(key, out object value))
            {
                result = (bool)value;
                return true;
            }
            else
            {
                result = default(bool);
                return false;
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
            _lastModifiedDateTime = DateTime.UtcNow;
        }
        
        public void LogStatus()
        {
            Log.Info($"{nameof(LocalDatabase)}");
            Log.Info($"  LastModifiedDate(UTC): {_lastModifiedDateTime}");
            Log.Info($"  LastModifiedDate(Local): {_lastModifiedDateTime.ToLocalTime()}");
            Log.Info($"  SavePath: {_savePath}");
        }
    }
}
