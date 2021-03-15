// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Rano.Addressable
{
    /// <example>
    /// AssetManager assetManager;
    /// 
    /// assetManager.LoadAsset(new Label("materials"));
    /// assetManager.IsLoaded(new Label("materials"));
    /// assetManager.IsLoaded(new Address("Assets/Materials/M_Red.mat"));
    ///
    /// Material red;
    /// red = assetManager.Get<Material>("Assets/Materials/M_Red.mat");
    ///
    /// GameObject enemy;
    /// enemy = assetManager.Instantiate("Assets/Prefabs/P_Enemy.prefab");
    /// <examples>

    public interface IAssetManager
    {
        void DownloadAssetAsync(Address address);
        void DownloadAssetAsync(Label label);
        void ClearCacheAsync(Address address);
        void ClearCacheAsync(Label label);

        void LoadAssetAsync(Address address);
        void LoadAssetsAsync(Label label);
        void UnloadAssetAsync(Address address);
        void UnloadAssetsAsync(Label label);
        
        T Get<T>(Address address);
        GameObject Instantiate(Address address);     
    }
}