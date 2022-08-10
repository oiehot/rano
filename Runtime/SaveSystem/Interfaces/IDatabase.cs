using System.Collections.Generic;

namespace Rano.SaveSystem
{
    public interface IDatabase
    {   
        public bool Load();
        public void Save();
        public void Clear();
        public string GetString(string key, string defaultValue = null);
        public void SetString(string key, string value);
        public Dictionary<string, object> GetDictionary(string key);
        public void SetDictionary(string key, Dictionary<string, object> value);        
        public bool HasKey(string key);
#if UNITY_EDITOR || DEVELOPMENT_BUILD        
        public bool UseResetOnStart { get; set; }
#endif        
    }
}
