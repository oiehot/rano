// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Text; // Encoding
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
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
        
        public static void SaveToJson<T>(string filePath, T data, bool prettyPrint=true)
        {
            // Newtonsoft.Json 을 사용하는 경우: string json = JsonConvert.SerializeObject(data);
            string txt = JsonUtility.ToJson(data, prettyPrint);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
            Save(filePath, bytes);
        }

        public static void SaveToBinary<T>(string filePath, T data)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = $"{Application.persistentDataPath}/{filePath}";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
        }
        
        public static byte[] Load(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException($"로드할 파일을 찾지 못했습니다: {filePath}");
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
                throw new System.IO.FileNotFoundException($"로드할 Json 파일을 찾지 못했습니다: {filePath}");
            }
            
            byte[] bytes;
            bytes = Load(filePath);
            string json = System.Text.Encoding.UTF8.GetString(bytes);

            return JsonUtility.FromJson<T>(json);
        }

        public static T LoadFromBinary<T>(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (System.IO.File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                T data = (T)formatter.Deserialize(stream);
                return data;
            }
            else
            {
                throw new System.IO.FileNotFoundException($"로드할 Json 파일을 찾지 못했습니다: {filePath}");
                // return null;
            }
        }
    }
}