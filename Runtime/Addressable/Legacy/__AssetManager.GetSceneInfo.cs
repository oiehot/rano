#if false

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
    public partial class AssetManager : MonoSingleton<AssetManager>
    {
        public SceneInfo GetSceneInfo(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.GetSceneInfo(path);
        }

        public SceneInfo GetSceneInfo(Address address)
        {
            return this.GetSceneInfo(address.GetPath());
        }

        /// <summary>씬 상태 정보를 얻는다</summary>
        public SceneInfo GetSceneInfo(Path path)
        {   
            if (this.scenes.ContainsKey(path))
            {
                return this.scenes[path];
            }
            return null;
        }

        public Path GetActiveScenePath()
        {
            Scene scene;
            scene = SceneManager.GetActiveScene();
            return new Path(scene.path);
        }

        public SceneInfo GetActiveSceneInfo()
        {
            Path path;
            path = this.GetActiveScenePath();
            return this.GetSceneInfo(path);
        }

        // public void DebugLogScenes()
        // {
        //     #if UNITY_EDITOR || DEVELOPMENT_BUILD
        //     Debug.Log("----");
        //     foreach (KeyValuePair<Path,SceneInfo> item in this.scenes)
        //     {
        //         SceneInfo sceneInfo = item.Value;
        //         Debug.Log($"{item.Value}");
        //     }
        //     #endif
        // }
    }
}

#endif