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
    public partial class AssetManager : MonoBehaviour
    {
        /// <summary>씬 상태 정보를 얻는다</summary>
        public SceneInfo GetSceneInfo(Path path)
        {   
            if (this.scenes.ContainsKey(path))
            {
                return this.scenes[path];
            }
            return null;
        }

        public void DebugLogScenes()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("----");
            foreach (KeyValuePair<Path,SceneInfo> item in this.scenes)
            {
                SceneInfo sceneInfo = item.Value;
                Debug.Log($"{item.Value}");
            }
            #endif
        }
    }
}