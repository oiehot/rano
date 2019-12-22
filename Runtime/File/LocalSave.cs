namespace Rano.File
{
    using System;
    using System.Text; // Encoding
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEditor;
    using Newtonsoft.Json; // JsonConvert
    
    // TODO: Module Test Require
    public static class LocalSave
    {
        public static void Save(string filePath, byte[] bytes)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            Debug.Log($"Save to {path}");
            System.IO.File.WriteAllBytes(path, bytes);
        }
        
        // TODO: Test Require
        public static void SaveToJson(string filePath, object data)
        {
            string json = JsonConvert.SerializeObject(data); // JsonConvert (Newtonsoft JSON)
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Save(filePath, bytes);
        }
        
        public static byte[] Load(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            Debug.Log($"Load from {path}");
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;

        }
        
        // TODO: Test Require
        public static T Load<T>(string filePath)
        {
            byte[] bytes;
            bytes = Load(filePath);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<T>(json);
        }
        
        // [MenuItem("Rano/File/Save Test")]
        // public static void SaveTest()
        // {
        //     Save("text.txt", Encoding.UTF8.GetBytes("Hello World!"));
        // }

        // [MenuItem("Rano/File/Load Test")]
        // public static void LoadTest()
        // {
        //     byte[] bytes = Load("text.txt");
        //     Debug.Log( Encoding.UTF8.GetString(bytes) );
        // }        
    }
}