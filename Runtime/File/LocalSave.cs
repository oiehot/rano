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
        #region Byte

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
    }
}