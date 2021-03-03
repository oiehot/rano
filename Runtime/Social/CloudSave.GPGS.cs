// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if (GPGS && UNITY_ANDROID)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Newtonsoft.Json; // JsonConvert

namespace Rano
{
    public enum CloudSaveRequestType
    {
        Save,
        Load
    }

    public class CloudSave
    {
        private Action<SavedGameRequestStatus, byte[]> OnLoadComplete { get; set; }
        private Action<SavedGameRequestStatus, ISavedGameMetadata> OnSaveComplete { get; set; }
        private CloudSaveRequestType requestType; // 이벤트에서 Save/Load 인지 판단하는데 사용.
        private byte[] _bytes;

        /// <summary>
        /// OnSavedGameOpened > OnLoadComplete(callback) 순서로 호출된다.
        /// </summary>
        public void Load(string filePath, Action<SavedGameRequestStatus, byte[]> callback)
        {
            Log.Info("GPGS Load: Requested");
            requestType = CloudSaveRequestType.Load;
            OnLoadComplete = callback;
        
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                filePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                this.OnSavedGameOpened
            );
        }

        public void LoadFromJson<T>(string filePath, Action<SavedGameRequestStatus, T> userCallback)
        {
            Log.Info("GPGS LoadFromJson: Requested");
            Action<SavedGameRequestStatus, byte[]> motherCallback = (status, bytes) => {
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                T data = JsonUtility.FromJson<T>(json);
                userCallback(status, data);
            };
            Load(filePath, motherCallback);
        }

        /// <summary>
        /// OnSavedGameOpened > OnSaveComplete(userCallback) 순서로 호출된다.
        /// </summary>
        public void Save(string filePath, byte[] bytes, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            Log.Info("GPGS Save: Requested");
            this.requestType = CloudSaveRequestType.Save;
            this.OnSaveComplete = callback;
            this._bytes = bytes;

            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                filePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                this.OnSavedGameOpened
            );
        }

        public void SaveToJson<T>(string filePath, T data, Action<SavedGameRequestStatus, ISavedGameMetadata> userCallback)
        {
            Log.Info("GPGS SaveToJson: Requested");            
            string json;
            byte[] bytes;
            try
            {
                json = JsonUtility.ToJson(data);
            }
            catch (Exception e)
            {
                throw new Exception($"Object to Json String Failed: {e.Message}");
            }

            try
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(json);
            }
            catch (Exception e)
            {
                throw new Exception($"Json String to Bytes Failed: {e.Message}");
            }

            Save(filePath, bytes, userCallback);
        }

        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            Log.Info("GPGS OnSavedGameOpened");
            if (requestType == CloudSaveRequestType.Load)
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    Log.Info("GPGS ReadBinaryData");
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, this.OnLoadComplete);
                }
                else
                {
                    Log.Info("GPGS Call OnLoadComplete");
                    this.OnLoadComplete(status, null);
                }
            }
            else if (requestType == CloudSaveRequestType.Save)
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    Log.Info("GPGS CommitUpdate");
                    ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                    SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
                    savedGameClient.CommitUpdate(metadata, update, _bytes, this.OnSaveComplete);
                }
                else
                {
                    Log.Info("GPGS Call OnSaveComplete");
                    this.OnSaveComplete(status, metadata);
                }
            }
            _bytes = null;
        }
    }
}

#endif