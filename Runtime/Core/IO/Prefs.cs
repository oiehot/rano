using UnityEngine;

namespace Rano.IO
{
    public class PrefsBool
    {
        public string Key { get; private set; }
        public bool DefaultValue { get; private set; }

        public PrefsBool(string key, bool defaultValue = false)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public bool Value
        {
            get
            {
                return Prefs.GetBool(Key, DefaultValue);
            }
            set
            {
                Prefs.SetBool(Key, value);
            }
        }
    }

    public static class Prefs
    {
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key);
        }

        public static void SetBool(string key, bool b)
        {
            int i = b ? 1 : 0;
            PlayerPrefs.SetInt(key, i);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            int defaultInt = defaultValue ? 1 : 0;
            int i = PlayerPrefs.GetInt(key, defaultInt);
            return i != 0 ? true : false;
        }

#if false
        public static void SetToBinary<T>(string key, T value)
        {
            var binaryFormatter = new BinaryFormatter(); // System.Runtime.Serialization.Formatters.Binary
            var memoryStream = new MemoryStream(); // System.IO
            
            // value를 바이트 배열로 변환해서 메모리에 저장한다.
            binaryFormatter.Serialize(memoryStream, value);
            
            // System.Convert
            // UnityEngine.PlayerPrefs
            PlayerPrefs.SetString(key, Convert.ToBase64String(memoryStream.GetBuffer()) );
            
            // TODO: PlayerPrefs.세이브()
        }
        
        public static T GetFromBinary<T>(string key)
        {
            var data = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(data))
            {
                var binaryFormatter = new BinaryFormatter();
                var memoryStream = new MemoryStream( Convert.FromBase64String(data) );
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
            return default(T); // replace from return null;
        }
#endif
    }
}