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
        public bool IsAssetLoaded(Path path)
        {
            AssetInfo assetInfo;
            assetInfo = this.GetAssetInfo(path);
            if (assetInfo != null)
            {
                if (assetInfo.status == AssetStatus.Loaded) return true;
                else return false;
            }
            else return false;
        }

        public bool IsAssetLoaded(Address address)
        {
            return this.IsAssetLoaded(address.GetPath());
        }

        public bool IsAssetLoaded(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.IsAssetLoaded(path);
        }
    }
}

#endif