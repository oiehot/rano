// TODO:  뉴튼소프트 플러그인 연계 방법 정리.
namespace Rano.File
{
    using System;
    using System.Text; // Encoding
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEditor;
    using Newtonsoft.Json; // JsonConvert
    
    public static class LocalSave
    {
        public static void Save(string filePath, byte[] bytes)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            Debug.Log($"Save to {path}");
            System.IO.File.WriteAllBytes(path, bytes);
        }
        
        public static void SaveToJson(string filePath, object data)
        {
            string json = JsonConvert.SerializeObject(data); // JsonConvert (Newtonsoft JSON)
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Save(filePath, bytes);
        }
        
        public static byte[] Load(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path)) return null;
            
            Debug.Log($"Load from {path}");
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;

        }

        public static T LoadFromJson<T>(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path)) return default(T);
            
            byte[] bytes;
            bytes = Load(filePath);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<T>(json);
        }     
    }
}