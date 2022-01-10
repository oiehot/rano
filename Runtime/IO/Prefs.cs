// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
                return Rano.IO.Prefs.GetBool(Key, DefaultValue);
            }
            set
            {
                Rano.IO.Prefs.SetBool(Key, value);
            }
        }

        // 구조체인 경우 getter, setter 삭제하고 이 메소드를 사용.
        //public void SetValue(bool value)
        //{
        //    Rano.IO.Prefs.SetBool(Key, value);
        //}

        //public bool GetValue()
        //{
        //    return Rano.IO.Prefs.GetBool(Key, DefaultValue);
        //}
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