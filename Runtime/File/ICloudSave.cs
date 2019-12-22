namespace Rano.File
{
    using System;
	// using System.Collections;
	// using System.Collections.Generic;
	// using UnityEngine;
	// using UnityEngine.SocialPlatforms;
	using GooglePlayGames;
	using GooglePlayGames.BasicApi;
	using GooglePlayGames.BasicApi.SavedGame;
        
    interface ICloudSave
    {
        Action<SavedGameRequestStatus, byte[]> OnLoadComplete { get; set; }
        Action<SavedGameRequestStatus, ISavedGameMetadata> OnSaveComplete { get; set; }
        void Save(string filePath, byte[] bytes);
        void Load(string filePath);
    }
}