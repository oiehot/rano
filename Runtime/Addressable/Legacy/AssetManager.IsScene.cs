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
    public partial class AssetManager : MonoBehaviour
    {
        public bool IsSceneLoaded(Path path)
        {
            SceneInfo sceneInfo;
            sceneInfo = this.GetSceneInfo(path);
            if (sceneInfo != null)
            {
                if (sceneInfo.status == SceneStatus.Loaded) return true;
                else return false;
            }
            else return false;
        }

        public bool IsSceneLoaded(Address address)
        {
            return this.IsSceneLoaded(address.GetPath());
        }

        public bool IsSceneLoaded(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.IsSceneLoaded(path);
        }
    }
}

#endif