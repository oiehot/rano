// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rano.File
{
    public static class LocalSave
    {
        #region BYTE

        public static void Write(string filePath, byte[] bytes)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            System.IO.File.WriteAllBytes(path, bytes);
        }

        public static byte[] Read(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException($"로드할 파일을 찾지 못했습니다: {filePath}");
            }
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }

        #endregion

        #region JSON

        public static void SaveToJson(string filePath, object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            Write(filePath, bytes);
        }

        public static string GetStringFromJson(string filePath)
        {
            string path = $"{Application.persistentDataPath}/{filePath}";
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException($"로드할 Json 파일을 찾지 못했습니다: {filePath}");
            }
            byte[] bytes;
            bytes = Read(filePath);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static T GetObjectFromJson<T>(string filePath)
        {
            string jsonString = GetStringFromJson(filePath);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static dynamic GetDynamicFromJson(string filePath)
        {
            string jsonString = GetStringFromJson(filePath);
            return JObject.Parse(jsonString);
        }

        //public static T LoadFromJson<T>(string filePath)
        //{
        //    string path = $"{Application.persistentDataPath}/{filePath}";
        //    if (!System.IO.File.Exists(path))
        //    {
        //        throw new System.IO.FileNotFoundException($"로드할 Json 파일을 찾지 못했습니다: {filePath}");
        //    }

        //    byte[] bytes;
        //    bytes = Load(filePath);
        //    string json = System.Text.Encoding.UTF8.GetString(bytes);

        //    return JsonUtility.FromJson<T>(json);
        //}

        #endregion

        //public static void SaveToBinary<T>(string filePath, T data)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();

        //    string path = $"{Application.persistentDataPath}/{filePath}";
        //    FileStream stream = new FileStream(path, FileMode.Create);
        //    formatter.Serialize(stream, data);
        //    stream.Close();
        //}

        //public static T LoadFromBinary<T>(string filePath)
        //{
        //    string path = $"{Application.persistentDataPath}/{filePath}";
        //    if (System.IO.File.Exists(path))
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        FileStream stream = new FileStream(path, FileMode.Open);
        //        T data = (T)formatter.Deserialize(stream);
        //        return data;
        //    }
        //    else
        //    {
        //        throw new System.IO.FileNotFoundException($"로드할 Json 파일을 찾지 못했습니다: {filePath}");
        //        // return null;
        //    }
        //}
    }
}