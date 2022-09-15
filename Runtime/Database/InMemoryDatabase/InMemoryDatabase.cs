#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.IO;

namespace Rano.Database
{
    [Serializable]
    public struct InMemoryDatabaseData
    {
        public DateTime lastModifiedDateTime;
    }
    
    public sealed class InMemoryDatabase : ILocalDatabase
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
        private readonly string _jsonSavePath;
        private DateTime _lastModifiedDateTime;
        
        public bool IsInitialized => _state >= EState.Ready;
        public DateTime LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => _lastModifiedDateTime = value;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private readonly PrefsBool _resetOnStartPref = new PrefsBool($"{nameof(InMemoryDatabase)}.ResetOnStart");
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
            _state = EState.None;
            _dict = new Dictionary<string, object>();
            _savePath = $"{Application.persistentDataPath}/save";
            _jsonSavePath = $"{_savePath}.json";
            
            Initialize();
        }
        
        private object CaptureSystemData()
        {
            var state = new InMemoryDatabaseData
            {
                lastModifiedDateTime = _lastModifiedDateTime
            };
            return state;
        }
        
        private void RestoreSystemData(object state)
        {
            InMemoryDatabaseData data = (InMemoryDatabaseData)state;
            _lastModifiedDateTime = data.lastModifiedDateTime;
        }

        private void Initialize()
        {
            Log.Sys($"{typeof(InMemoryDatabase).ToString()}: Initializing...", caller: false);
            Log.Info($"  SavePath: {_savePath}");
            Log.Info($"  JsonSavePath: {_jsonSavePath}");
            
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
                _state = Load() ? EState.Ready : EState.None;
            }
            else
            {
                _state = EState.Ready;
            }
#else
            _state = Load() ? EState.Ready : EState.None;
#endif
        }
        
        public bool LoadFromBinary(byte[] bytes)
        {
            object? data;
            Dictionary<string, object> dict;
            
            Log.Info($"로드 중... ({bytes.Length} bytes)");
            
            // 세이브 데이터를 오브젝트로 변환한다.
            data = Rano.Encoding.Binary.ConvertBinaryToObject(bytes);
            if (data == null)
            {
                Log.Warning($"바이트 데이터를 오브젝트로 변환하는데 실패");
                return false;
            }

            // 오브젝트를 사전으로 변환한다.
            try
            {
                dict = (Dictionary<string, object>)data;
            }
            catch (Exception e)
            {
                Log.Warning($"오브젝트를 사전으로 변환하는데 실패");
                Log.Exception(e);
                return false;
            }

            // 기존 사전을 비우고 로드한다.
            _dict.Clear();
            _dict = dict;
            
            // 데이터베이스 시스템 정보를 복구한다.
            if (_dict.TryGetValue(SYSTEM_DATA_KEY, out object systemData) == true)
            {
                RestoreSystemData(systemData);
            }
            
            Log.Info($"로드 성공 ({bytes.Length} bytes)");
            return true;
        }

        public bool LoadFromBinaryFile(string filePath)
        {
            // 파일 체크
            bool fileExists;
            try
            {
                fileExists = System.IO.File.Exists(_savePath);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            if (fileExists == false)
            {
                Log.Info($"파일을 찾을 수 없음 ({filePath})");
                return false;
            }

            // 파일 읽기
            Log.Info($"읽는 중... ({filePath})");
            byte[]? bytes;
            try
            {
                bytes = Rano.IO.LocalFile.ReadBytes(filePath);
            }
            catch (Exception e)
            {
                Log.Warning($"읽기 오류 ({filePath})");
                Log.Exception(e);
                return false;
            }

            // 읽어온 데이터로 로드
            if (bytes != null)
            {
                Log.Info($"읽기 성공 ({filePath}, {bytes.Length} bytes)");
                return LoadFromBinary(bytes);
            }
            else
            {
                Log.Warning($"읽기 실패 ({filePath})");
                return false;
            }
        }

        public bool Load()
        {
            return LoadFromBinaryFile(_savePath);
        }

        /// <summary>
        /// 데이터베이스 전체를 바이너리 데이터로 리턴한다.
        /// </summary>
        public byte[]? GetArchive()
        {
            Log.Todo("압축 및 암호화 필요.");
            
            // 데이터베이스 시스템 정보를 업데이트한다.
            object systemData = CaptureSystemData();
            _dict[SYSTEM_DATA_KEY] = systemData;
            
            byte[]? bytes = Rano.Encoding.Binary.ConvertObjectToBinary(_dict);
            return bytes;
        }
        
        public bool SaveAsBinaryFile(string filePath)
        {
            Log.Info($"저장 중... ({filePath})");
            
            string tmpPath = $"{filePath}.tmp";
            
            // 데이터를 바이트로 변환한다.
            byte[]? bytes = GetArchive();
            if (bytes == null)
            {
                Log.Warning($"바이너리 아카이브에 실패");
                return false;
            }

            // 데이터를 파일에 쓰기.
            if (Rano.IO.LocalFile.WriteBytes(tmpPath, bytes) == false)
            {
                Log.Warning($"파일 쓰기 실패 ({tmpPath})");
                return false;
            }
            
            // 임시파일을 정식파일로 이동.
            Rano.IO.LocalFile.Move(tmpPath, filePath, true);

            Log.Info($"저장 완료 ({filePath}, {bytes.Length} bytes)");
            return true;
        }

        // public bool SaveAsJsonFile(string filePath)
        // {
        //     // 개발판에서는 json 파일도 저장한다.
        //     Log.Info($"{filePath} 저장중...");
        //     Log.Todo("파일스트림을 이용해서 저장해야함.");
        //     var jsonString = Rano.Encoding.Json.ConvertObjectToString(_dict);
        //     Rano.IO.LocalFile.WriteString(filePath, jsonString);
        //     Rano.IO.LocalFile.Move(_temporaryJsonSavePath, _jsonSavePath, true);
        // }

        public bool Save()
        {
            return SaveAsBinaryFile(_savePath);
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
            Log.Info($"{nameof(InMemoryDatabase)}");
            Log.Info($"  LastModifiedDate(UTC): {_lastModifiedDateTime}");
            Log.Info($"  SavePath: {_savePath}");
            Log.Info($"  JsonSavePath: {_jsonSavePath}");            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            Log.Info($"  ResetOnStart Key: {_resetOnStartPref.Key}");
            Log.Info($"  {nameof(UseResetOnStart)}: {UseResetOnStart}");
#endif
        }
    }
}
