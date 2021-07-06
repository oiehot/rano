#if false

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;
using VoxelBusters.EssentialKit;

namespace Rano.Platform
{
    /// <ref>https://assetstore.essentialkit.voxelbusters.com</ref>
    public partial class DataManager : MonoSingleton<DataManager>
    {
        void OnEnable()
        {
            Log.Info("DataManager Enabled");
            CloudServices.OnUserChange              += OnUserChange;
            CloudServices.OnSavedDataChange         += OnSavedDataChange;
            CloudServices.OnSynchronizeComplete     += OnSynchronizeComplete;
        }

        void OnDisable()
        {
            CloudServices.OnUserChange              -= OnUserChange;
            CloudServices.OnSavedDataChange         -= OnSavedDataChange;
            CloudServices.OnSynchronizeComplete     -= OnSynchronizeComplete;
            Log.Info("DataManager Disabled");
        }

        public void Sync()
        {
            Log.Info("DataManager Sychronize Requested");
            CloudServices.Syncronize();
        }

        /// <summary>
        /// 클라우드 계정이 변경되면 호출된다.
        /// </summary>
        void OnUserChange()
        {
            Log.Info("DataManager Account Changed");
        }

        /// <summary>
        /// 클라우드 데이터가 변경되면 호출된다.
        /// </summary>
        /// <ref>
        /// CloudSavedDataChangeReasonCode.ServerChange
        /// CloudSavedDataChangeReasonCode.InitialSyncChange
        /// CloudSavedDataChangeReasonCode.QuotaViolation
        /// CloudSavedDataChangeReasonCode.AccountChange
        /// </refs>
        void OnSavedDataChange(CloudSavedDataChangeReasonCode reason)
        {
            Log.Important($"DataManager Saved Data Changed: {reason}");
        }

        /// <summary>
        /// 동기화 명령이 완료된 뒤 호출된다.
        /// </summary>
        void OnSynchronizeComplete(CloudServicesSynchronizeResult result)
        {
            Log.Important($"DataManager Sychronize Completed: {result}"); // result.Success
            // TODO: By this time, you have the latest data from cloud and you can start reading.
        }

        /// <summary>
        /// 클라우드에 데이터를 저장한다.
        /// </summary>
        public void Save(string key, string value)
        {
            CloudServices.SetString(key, value);
        }

        /// <summary>
        /// 클라우드에 데이터를 저장한다.
        /// </summary>
        public void Save<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            Save(key, json);
        }

        /// <summary>
        /// 클라우드에서 동기화된 데이터를 로드한다.
        /// </summary>
        public string Load(string key)
        {
            string value = CloudServices.GetString(key);
            return value;
        }

        /// <summary>
        /// 클라우드에서 동기화된 데이터를 로드한다.
        /// </summary>
        public T Load<T>(string key)
        {
            string json = Load(key);
            T data = JsonUtility.FromJson<T>(json);
            return data;
        }
    }
}

#endif