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
        public T GetAsset<T>(Path path)
        {
            if (this.assets.ContainsKey(path))
            {
                AssetInfo assetInfo = this.assets[path];
                if (assetInfo.status == AssetStatus.Loaded)
                {
                    return (T)assetInfo.asset;
                }
                else
                {
                    throw new Exception($"리소스가 로드되어 있지 않음: {path}");
                }
            }
            else
            {
                throw new Exception($"리소스가 로드되어 있찌 않음: {path}");
            }
        }

        /// <summary>Address를 통해 캐싱된 에셋을 얻는다</summary>
        public T GetAsset<T>(Address address)
        {
            Path path = address.GetPath();
            return this.GetAsset<T>(path);
        }

        /// <summary>AssetReference를 통해 캐싱된 에셋을 얻는다</summary>
        public T GetAsset<T>(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.GetAsset<T>(path);
        }
    }
}