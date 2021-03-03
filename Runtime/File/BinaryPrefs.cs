// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace Rano.File
{   
    public static class BinaryPrefs<T>
    {
        /// var data = new SerializableObject(params);
        /// Rano.Data.BinaryPrefs<T>.Set("core", data);
        public static void Set(string key, T value)
        {
            var binaryFormatter = new BinaryFormatter(); // System.Runtime.Serialization.Formatters.Binary
            var memoryStream = new MemoryStream(); // System.IO
            
            // value를 바이트 배열로 변환해서 메모리에 저장한다.
            binaryFormatter.Serialize(memoryStream, value);
            
            // System.Convert
            // UnityEngine.PlayerPrefs
            PlayerPrefs.SetString( key, Convert.ToBase64String(memoryStream.GetBuffer()) );
            
            // TODO: PlayerPrefs.세이브()
        }
        
        /// var data = Rano.Data.BinaryPrefs<T>.Get("core");
        public static T Get(string key)
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
    }
}

