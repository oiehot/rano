#nullable enable

using System.Collections.Generic;

namespace Rano.SaveSystem
{
    public interface IDatabase
    {   
        public bool Load();
        public void Save();
        public void Clear();
        public string? GetString(string key, string? defaultValue = null); // TODO: 옵셔널 처리, 구현체들
        public void SetString(string key, string value);
        public bool GetBool(string key);
        public bool TryGetBool(string key, out bool result);
        public void SetBool(string key, bool value);
        public Dictionary<string, object>? GetDictionary(string key); // TODO: 옵셔널 처리, 구현체들
        public void SetDictionary(string key, Dictionary<string, object> value);        
        public bool HasKey(string key);
#if UNITY_EDITOR || DEVELOPMENT_BUILD        
        public bool UseResetOnStart { get; set; }
#endif        
    }
}
