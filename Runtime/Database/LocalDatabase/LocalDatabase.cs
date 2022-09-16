#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.IO;

namespace Rano.Database
{
    [Serializable]
    public struct LocalDatabaseData
    {
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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private readonly PrefsBool _resetOnStartPref = new PrefsBool($"{nameof(LocalDatabase)}.ResetOnStart");
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

        public LocalDatabase()
        {
            _state = EState.None;
            _dict = new Dictionary<string, object>();
            _savePath = $"{Application.persistentDataPath}/save";
            
            Initialize();
        }
        
        private object CaptureSystemData()
        {
            var state = new LocalDatabaseData
            {
                lastModifiedDateTime = _lastModifiedDateTime
            };
            return state;
        }
        
        private void RestoreSystemData(object state)
        {
            LocalDatabaseData data = (LocalDatabaseData)state;
            _lastModifiedDateTime = data.lastModifiedDateTime;
        }

        private void Initialize()
        {
            Log.Sys($"{typeof(LocalDatabase).ToString()}: Initializing...", caller: false);
            Log.Info($"  SavePath: {_savePath}");
            
            _state = EState.Initializing;
            _dict.Clear();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            bool useLoad = true;
            
            if (UseResetOnStart)
            {
                Log.Important($"ResetOnStart 플래그가 켜져있어 로드하지 않고 시작합니다.");
                useLoad = false;
            }
            
            if (useLoad)
            {
                // 로드 실패시 기본값 사용.
                if (Load() == false)
                {
                    _dict.Clear();
                }
            }
            _state = EState.Ready;
#else 
            // 로드 실패시 기본값 사용.
            if (Load() == false)
            {
                _dict.Clear();
            }
            _state = EState.Ready;
#endif
        }

        public bool Load()
        {
            // return LoadFromBinaryFile(_savePath);
            return LoadFromJsonFile(_savePath);
        }

        public bool Save()
        {
            // return SaveAsBinaryFile(_savePath);
            return SaveAsJsonFile(_savePath);
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
            Log.Info($"  SavePath: {_savePath}");           
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log.Info($"  ResetOnStart Key: {_resetOnStartPref.Key}");
            Log.Info($"  {nameof(UseResetOnStart)}: {UseResetOnStart}");
#endif
        }
    }
}
