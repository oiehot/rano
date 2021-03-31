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
        #region LoadAsset

        // WARN: Label 에셋 레퍼런스는 사용할 수 없음
        public AsyncOperationHandle LoadAssetAsync(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.LoadAssetAsync(path);
        }

        // TODO: 어떻게 AsyncOperationHandle을 리턴할 수 있을까?
        public void LoadAssetAsync(Label label)
        {
            Path[] pathArray = label.GetPath();
            for (int i=0; i<pathArray.Length; i++)
            {
                this.LoadAssetAsync(pathArray[i]);
            }
        }

        public AsyncOperationHandle LoadAssetAsync(Address address)
        {
            return this.LoadAssetAsync(address.GetPath());
        }

        /// <summary>에셋을 로드한다</summary>
        public AsyncOperationHandle LoadAssetAsync(Path path)
        {
            AssetInfo assetInfo;
            AsyncOperationHandle handle;

            if (!path.IsAssetPath())
            {
                throw new Exception($"에셋 로드 실패: {path}는 에셋이 아님");
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

            // 구조체인 AsyncOperationHandle은 null 리턴할 수 없으므로
            // 이미 로드되어 있더라도 로드를 요청하도록 진행.
            // 여러 번들(레이블)이 로드될 때 중복로드되는 경우가 발생할것임.
            // if (assetInfo.status == AssetStatus.Loaded)
            // {
            //     // TODO: Label 중복 로드 문제시 자주 발생할텐데?
            //     // throw new Exception($"에셋이 이미 로드되어 있어 로드 실패: {assetInfo.path}");
            //     return handle; // TODO: 초기화 되지 않은 handle을 리턴시 코루틴 yield가 정상적으로 작동될까?
            // }

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

            StartCoroutine(this.UpdateAssetProgressCoroutine(assetInfo, handle));

            return handle;
        }

        #endregion

        #region UnloadAsset

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

        #endregion

        #region GetAsset

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
                throw new Exception($"리소스가 로드되어 있지 않음: {path}");
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

        #endregion

        #region GetAssetInfo

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

        #endregion        
    }
}