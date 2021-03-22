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
        public void LoadAssetAsync(Path path)
        {
            if (this.pathToId.ContainsKey(path) == true)
            {
                AsyncOperationHandle handle;
                string key = this.pathToId[path];
                handle = Addressables.LoadAssetAsync<object>(key);
                handle.Completed += (AsyncOperationHandle handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"리소스 로드 성공: {path}");
                        this.assets.Add(path, handle.Result); // 캐싱
                    }
                    else
                    {
                        Log.Warning($"리소스 로드 실패: {path}");
                    }
                };
            }
            else
            {
                throw new Exception($"{path}에 해당하는 어드레서블을 찾을 수 없음");
            }
        }

        public void LoadAssetAsync(Address address)
        {
            AsyncOperationHandle handle;
            handle = Addressables.LoadAssetAsync<object>(address.value);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Path path = address.GetPath();
                    Log.Info($"리소스 로드 성공: {address}");
                    this.assets.Add(path, handle.Result); // 캐싱
                }
                else
                {
                    Log.Warning($"리소스 로드 실패: {address}");
                }
            };
        }

        public void LoadAssetAsync(AssetReference assetReference)
        {
            AsyncOperationHandle handle;
            handle = Addressables.LoadAssetAsync<object>(assetReference);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Path path = AssetManagerUtils.GetPath(assetReference);
                    Log.Info($"리소스 로드 성공: {path} <= {assetReference.RuntimeKey}");
                    this.assets.Add(path, handle.Result); // 캐싱
                }
                else
                {
                    Log.Warning($"리소스 로드 실패: {assetReference.RuntimeKey}");
                }
            };
        }        
        
        /// <todo>
        /// 순차적으로 로드하게 수정해야함.
        /// 다수에셋 로드:
        /// Addressables.LoadAssetsAsync<TObject>('key' 또는 'IResourceLocation', 'Action<TObject> callback')
        ///     key에 해당하는 에셋들을 모두 로드한다. (callback은 각 개체가 로드될 때마다 호출된다.)
        /// </todo>
        public void LoadAssetAsync(Label label)
        {
            // TODO: LoadResourceLocationsAsync 없이 내부 캐싱된 데이터를 사용할것
            Addressables.LoadResourceLocationsAsync(label.value)
                .Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (var item in handle.Result)
                        {
                            this.LoadAssetAsync(new Path(item.InternalId));
                        }             
                    }
                    else
                    {
                        Log.Info($"리소스 로드 실패: {label.value} (label)");
                    }
                    // TODO: Release?
                    // Addressables.Release(handle);
                };
        }

        public void LoadAssetAsyncB(Label label)
        {
            AsyncOperationHandle handle;
            handle = Addressables.LoadAssetAsync<object>(label.value);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"리소스 로드 성공: {label}");
                    // this.assets.Add(path, handle.Result); // 캐싱
                }
                else
                {
                    Log.Warning($"리소스 로드 실패: {label}");
                }
            };
        }

        public void UnloadAsset(Path path)
        {
            if (this.assets.ContainsKey(path))
            {
                Log.Info($"리소스 언로드: {path}");
                Addressables.Release(this.assets[path]);
                this.assets.Remove(path); // 캐시 삭제
            }
            else
            {
                throw new Exception($"이미 언로드되어 있음: {path}");
            }
        }

        public void UnloadAsset(Address address)
        {
            Path path = address.GetPath();
            this.UnloadAsset(path);
        }

        public void UnloadAsset(Label label)
        {
            Log.Info($"리소스 언로드: {label}");

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

#endif