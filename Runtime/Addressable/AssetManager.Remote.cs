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
    public partial class AssetManager : MonoBehaviour, IAssetManager
    {
        /// <summary>
        /// 다운로드 사이즈 얻기. 0이면 이미 캐싱된 것이다.
        /// </summary>
        public void GetDownloadSizeAsync(Label label)
        {
            Addressables.GetDownloadSizeAsync(label.value)
                .Completed += (AsyncOperationHandle<long> handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"Download Size: {label.value}: {handle.Result}");
                    }
                    else
                    {
                        Log.Warning($"Get Download Size Failed: {label.value}");
                    }
                    Addressables.Release(handle);
            };
        }

        /// <summary>관련된 리소스 로케이터들을 얻는다</summary>
        public void GetResourceLocationsAsync(Label label)
        {
            Addressables.LoadResourceLocationsAsync(label.value)
                .Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"Get ResourceLocations Complete from Label: {label.value}");
                        foreach (var item in handle.Result)
                        {
                            Log.Info($"* {item}");
                            Log.Info($"\tHasDependencies: {item.HasDependencies}");
                            Log.Info($"\tInternalId: {item.InternalId}");
                            Log.Info($"\tProviderId: {item.ProviderId}");
                        }
                    }
                    else
                    {
                        Log.Info($"Get ResourceLocations Failed from Label: {label.value}");
                    }
                    Addressables.Release(handle);
                };
        }

        /// <summary>관련된 모든 에셋번들을 다운로드함</summary>
        public void DownloadAssetAsync(Label label)
        {
            Addressables.DownloadDependenciesAsync(label.value)
                .Completed += (AsyncOperationHandle handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"DownloadDependencies Completed: {label.value} (using Label)");
                    }
                    else
                    {
                        Log.Warning($"DownloadDependencies Failed: {label.value} (using Label)");
                    }
                    // handle.PercentComplete;
                    Addressables.Release(handle);
                };
        }

        /// <summary>어드레스와 관련된 에셋번들들을 다운로드함</summary>
        public void DownloadAssetAsync(Address address)
        {
            Addressables.DownloadDependenciesAsync(address.value)
                .Completed += (AsyncOperationHandle handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"DownloadDependencies Completed: {address.value} (using Address)");
                    }
                    else
                    {
                        Log.Warning($"DownloadDependencies Failed: {address.value} (using Address)");
                    }
                    // handle.PercentComplete;
                    Addressables.Release(handle);
                };
        }
        
        /// <summary>주어진 레이블이 의존하는 에셋번들 캐시들을 삭제함</summart>
        public void ClearCacheAsync(Label label)
        {
            Addressables.ClearDependencyCacheAsync(label.value);
        }

        /// <summary>주어진 어드레스가 의존하는 에셋번들 캐시들을 삭제함</summart>
        public void ClearCacheAsync(Address address)
        {
            Addressables.ClearDependencyCacheAsync(address.value);
        }
    }
}