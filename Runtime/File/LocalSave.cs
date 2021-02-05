using System;
using System.Text; // Encoding
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using Newtonsoft.Json;

namespace Rano.File
{
    public static class LocalSave
    {
        public static void Save(string filePath, byte[] bytes)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            Log.Info($"Save to {path}");
            System.IO.File.WriteAllBytes(path, bytes);
        }
        
        public static void SaveToJson<T>(string filePath, T data)
        {
            // Newtonsoft.Json 을 사용하는 경우: string json = JsonConvert.SerializeObject(data);
            string txt = JsonUtility.ToJson(data, false); // prettyPrint: True
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
            Save(filePath, bytes);
        }
        
        public static byte[] Load(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path))
            {
                throw new Exception("File is not exists");
            }
            
            Log.Info($"Load from {path}");
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }

        public static T LoadFromJson<T>(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path))
            {
                throw new Exception("File is not exists");
            }
            
            byte[] bytes;
            bytes = Load(filePath);
            string json = System.Text.Encoding.UTF8.GetString(bytes);

            return JsonUtility.FromJson<T>(json);
        }     
    }
}