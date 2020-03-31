#if GooglePlayGames

namespace Rano.File
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SocialPlatforms;
	using GooglePlayGames;
	using GooglePlayGames.BasicApi;
	using GooglePlayGames.BasicApi.SavedGame;
    using Newtonsoft.Json; // JsonConvert
        
    public enum CloudSaveRequestType
    {
        Save,
        Load
    }
    
    // cloud = new CloudSave()
    // cloud.OnSaveComplete.Subscribe( (s, metdata) => {...} );
    // cloud.Save('path', '...')
    // cloud.Load('path', '...')
    public class CloudSave
    {
        public Action<SavedGameRequestStatus, byte[]> OnLoadComplete { get; set; }
        public Action<SavedGameRequestStatus, ISavedGameMetadata> OnSaveComplete { get; set; }
        
        private CloudSaveRequestType requestType; // 이벤트에서 Save/Load 인지 판단하는데 사용.
        private byte[] _bytes;   
        
        public void Save(string filePath, byte[] bytes, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            OnSaveComplete = callback;
            requestType = CloudSaveRequestType.Save; // OnSavedGameOpened에서 Save?Load 를 판정하기 위함.
            _bytes = bytes; // 나중에 이벤트 안에서 사용하기 위해 보관.
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                filePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                this.OnSavedGameOpened
            ); // 요청이 끝나면 OnSavedGameOpened가 호출된다.
        }
        
        public void SaveToJson(string filePath, object data, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            Save(filePath, bytes, callback);
        }
        
        // 저장2. 저장을 요청하기
        private void SaveCloud(ISavedGameMetadata metadata, byte[] bytes)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
            savedGameClient.CommitUpdate(metadata, update, bytes, this.OnSaveComplete);
        }
        
        // 로드1. 저장된 파일을 열기
        public void Load(string filePath, Action<SavedGameRequestStatus, byte[]> callback)
        {
            OnLoadComplete = callback;
            requestType = CloudSaveRequestType.Load;
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                filePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                this.OnSavedGameOpened
            ); // 요청이 끝나면 OnSavedGameOpened가 호출된다.
        }
        
        public void LoadFromJson<T>(string filePath, Action<SavedGameRequestStatus, T> callback)
        {
            Action<SavedGameRequestStatus, byte[]> cb = (status, bytes) => {
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                T data = JsonUtility.FromJson<T>(json);
                callback(status, data);
            };
            Load(filePath, cb);
        }
        
        // 로드 2. 파일 다운로드 요청
        private void LoadCloud(ISavedGameMetadata data)
        {
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(data, this.OnLoadComplete);
        }
    
        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                if (requestType == CloudSaveRequestType.Save) SaveCloud(metadata, _bytes);
                else if (requestType == CloudSaveRequestType.Load) LoadCloud(metadata);
            }
            _bytes = null;
        }
    }
}

#endif