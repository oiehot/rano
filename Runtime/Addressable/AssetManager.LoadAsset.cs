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
        public void LoadAsset(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            this.LoadAsset(path);
        }

        public void LoadAsset(Label label)
        {
            Path[] pathArray = label.GetPath();
            for (int i=0; i<pathArray.Length; i++)
            {
                this.LoadAsset(pathArray[i]);
            }
        }

        public void LoadAsset(Address address)
        {
            this.LoadAsset(address.GetPath());
        }

        /// <summary>에셋을 로드한다</summary>
        public void LoadAsset(Path path)
        {
            AssetInfo assetInfo;
            AsyncOperationHandle handle;

            if (!path.IsAssetPath())
            {
                // throw new Exception($"에셋 로드 실패: {path} 는 에셋이 아님");
                return;
            }            

            if (this.assets.ContainsKey(path) == true)
            {
                assetInfo = this.assets[path];
            }
            else
            {
                if (this.pathToId.ContainsKey(path) == false)
                {
                    throw new Exception($"에셋 로드 실패: {path}에 해당하는 어드레서블을 찾을 수 없음");
                }
                assetInfo = new AssetInfo();
                assetInfo.id = this.pathToId[path]; // TODO: Label면 절대로 안된다.
                assetInfo.path = path;
                assetInfo.asset = null;
                assetInfo.status = AssetStatus.None;
                this.assets.Add(path, assetInfo);
            }

            if (assetInfo.status == AssetStatus.Loaded)
            {
                // throw new Exception($"에셋이 이미 로드되어 있어 로드 실패: {assetInfo.path}");
                return;
            }

            assetInfo.status = AssetStatus.Loading;
            handle = Addressables.LoadAssetAsync<object>(assetInfo.id);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"에셋 로드 성공: {assetInfo.path}");
                    assetInfo.asset = handle.Result;
                    assetInfo.percent = 1.0f;
                    assetInfo.status = AssetStatus.Loaded;
                }
                else
                {
                    assetInfo.status = AssetStatus.Failed;
                    throw new Exception($"에셋 로드 실패: {assetInfo.path}");
                }
            };

            StartCoroutine(UpdateAssetProgressCoroutine(assetInfo, handle));
        }
    }
}