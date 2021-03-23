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
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Rano.Addressable
{
    public partial class AssetManager : MonoBehaviour
    {
        public void UnloadAsset(Address address)
        {
            Path path = address.GetPath();
            this.UnloadAsset(path);
        }

        public void UnloadAsset(Label label)
        {
            Path[] pathArray;
            pathArray = label.GetPath();
            for (int i=0; i<pathArray.Length; i++)
            {
                this.UnloadAsset(pathArray[i]);
            }
        }

        public void UnloadAsset(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            this.UnloadAsset(path);
        }
        
        /// <summary>에셋을 언로드한다</summary>
        public void UnloadAsset(Path path)
        {
            AssetInfo assetInfo;

            if (!path.IsAssetPath())
            {
                // throw new Exception($"에셋 언로드 실패: {path} 는 에셋이 아님");
                return;
            }      

            if (this.assets.ContainsKey(path) == true)
            {
                assetInfo = this.assets[path];
                if (assetInfo.status != AssetStatus.Loaded)
                {
                    // Log.Info($"에셋 언로드: {path} (이미 언로드되어 있어 생략함)");
                    return;
                }
            }
            else
            {
                // Log.Info($"에셋 언로드: {path} (이미 언로드되어 있어 생략함)");
                return;
            }

            Log.Info($"에셋 언로드: {path}");
            Addressables.Release(assetInfo.asset);
            assetInfo.asset = null;
            assetInfo.percent = 0.0f;
            assetInfo.status = AssetStatus.None;
        }

        /// <summary>
        /// Ref-Count가 0지만 언로드되지 않은 에셋은 강제로 언로드한다.
        /// Ref-Count가 0더라도, 에셋번들이 완전히 Unload 되지 않은 경우 Asset은 Unload되지 않는다.
        /// 주의: 잠깐 멈출 수 있음.
        /// </summary>
        public void FreeUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}