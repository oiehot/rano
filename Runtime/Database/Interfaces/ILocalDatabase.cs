#nullable enable

using System;
using System.Collections.Generic;

namespace Rano.Database
{
    public interface ILocalDatabase
    {
        public bool IsInitialized { get; }
        public DateTime LastModifiedDateTime { get; set; }
        public void Initialize();
        public bool Load();
        public bool LoadFromArchive(byte[] bytes);
        public bool Save();
        public void Clear();
        public byte[]? GetArchive();
        public string? GetString(string key, string? defaultValue = null); // TODO: 옵셔널 처리, 구현체들
        public void SetString(string key, string value);
        public bool GetBool(string key);
        public bool TryGetBool(string key, out bool result);
        public void SetBool(string key, bool value);
        public Dictionary<string, object>? GetDictionary(string key); // TODO: 옵셔널 처리, 구현체들
        public void SetDictionary(string key, Dictionary<string, object> value);        
        public bool HasKey(string key);
    }
}
