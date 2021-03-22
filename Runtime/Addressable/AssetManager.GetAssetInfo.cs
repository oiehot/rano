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
        public AssetInfo GetAssetInfo(Path path)
        {
            if (this.assets.ContainsKey(path))
            {
                return this.assets[path];
            }
            else
            {
                return null;
            }
        }

        public AssetInfo GetAssetInfo(Address address)
        {
            Path path;
            path = address.GetPath();
            return this.GetAssetInfo(path);
        }

        public AssetInfo GetAssetInfo(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.GetAssetInfo(path);
        }
    }
}