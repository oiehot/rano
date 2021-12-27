#if false

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano.PlatformServices.Cloud;

namespace Rano.SaveSystem
{
    public sealed class InMemorySyncer : MonoBehaviour
    {
        public string LastModifiedDateField => "__LastModifiedDate";

        [ContextMenu("Sync", false, 1001)]
        public void Sync()
        {
            StartCoroutine(nameof(SyncCoroutine));
        }

        public IEnumerator SyncCoroutine()
        {
            yield return CloudManager.Instance.SynchronizeCoroutine();

            // 수정날짜를 얻고 비교한다.
            string localStr;
            string cloudStr;
            DateTime localDateTime;
            DateTime cloudDateTime;
            localStr = InMemoryDatabase.Instance.GetString(LastModifiedDateField);
            cloudStr = CloudManager.Instance.GetString(LastModifiedDateField);
            localDateTime = localStr.ToDateTime();
            cloudDateTime = cloudStr.ToDateTime();

            if (localDateTime >= cloudDateTime)
            {
                Log.Important("InMemoryDatabase => CloudLocalCopy");
                CopyMemoryToCloud();
            }
            else
            {
                Log.Important("CloudLocalCopy => InMemoryDatabase");
                CopyCloudToMemory();
            }

            yield return CloudManager.Instance.SynchronizeCoroutine();
        }

        [ContextMenu("Copy Memory To Cloud",false,1002)]
        private void CopyMemoryToCloud()
        {
            Log.Info("CopyMemoryToCloud 시작");
            foreach (var key in InMemoryDatabase.Instance.Dict.Keys)
            {
                var t = InMemoryDatabase.Instance.Dict[key].GetType();

                Log.Info($"Key:{key} Type: {t}");
                if (InMemoryDatabase.Instance.Dict[key].GetType() == typeof(Dictionary<string, object>))
                {
                    Log.Info($"Key:{key} ValueType: Dictionary<string,object>");
                    byte[] bytes = null;
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, InMemoryDatabase.Instance.Dict[key]);
                        bytes = ms.ToArray();
                    }
                    Log.Info($"Set {key} to CloudLocalCopy");
                    CloudManager.Instance.SetByteArray(key, bytes);

                    byte[] testBytes = null;
                    testBytes = CloudManager.Instance.GetByteArray(key);

                    if (bytes.Length == testBytes.Length)
                    {
                        Log.Info("=> SameLength => Success");
                    }
                }
                else
                {
                    Log.Info($"Key:{key} ValueType: unknown => Skip");
                }
            }
        }

        [ContextMenu("Copy Cloud To Memory", false, 1003)]
        private void CopyCloudToMemory()
        {
            Log.Info("CopyCloudToMemory 시작");
            InMemoryDatabase.Instance.Clear();
            IDictionary cloudDict = CloudManager.Instance.GetDict();

            foreach (object keyObject in cloudDict.Keys)
            {
                object value = cloudDict[keyObject];
                string key = (string)keyObject;
                Log.Info($"Key: {key}, ValueType:{value.GetType()}");
                if (value.GetType() == typeof(byte[]))
                {
                    
                    byte[] bytes = CloudManager.Instance.GetByteArray(key);
                    var bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        object obj = bf.Deserialize(ms);
                        Log.Info($"Set {key} to InMemoryDatabase");
                        InMemoryDatabase.Instance.SetDict(key, (Dictionary<string,object>)obj);
                    }
                }
            }
        }

    }
}

#endif