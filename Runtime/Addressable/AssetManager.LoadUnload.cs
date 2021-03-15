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
        public void LoadAssetAsync(Address address)
        {
            AsyncOperationHandle handle;

            handle = Addressables.LoadAssetAsync<object>(address.value);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"리소스 로드 성공: {address.value} (address)");
                    this.assets.Add(address, handle.Result);
                }
                else
                {
                    Log.Warning($"리소스 로드 실패: {address.value} (address)");
                }
            };
        }
        
        /// <todo>
        /// 다수에셋 로드:
        /// Addressables.LoadAssetsAsync<TObject>('key' 또는 'IResourceLocation', 'Action<TObject> callback')
        ///     key에 해당하는 에셋들을 모두 로드한다. (callback은 각 개체가 로드될 때마다 호출된다.)
        /// </todo>
        public void LoadAssetsAsync(Label label)
        {
            Addressables.LoadResourceLocationsAsync(label.value)
                .Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (var item in handle.Result)
                        {
                            this.LoadAssetAsync(new Address(item.InternalId));
                            // Log.Info($"* {item}");
                            // Log.Info($"\tHasDependencies: {item.HasDependencies}");
                            // Log.Info($"\tInternalId: {item.InternalId}");
                            // Log.Info($"\tProviderId: {item.ProviderId}");
                        }             
                    }
                    else
                    {
                        Log.Info($"리소스 로드 실패: {label.value} (label)");
                    }
                    Addressables.Release(handle);
                };
        }


        public void UnloadAssetAsync(Address address)
        {
            if (this.assets.ContainsKey(address))
            {
                Addressables.Release(this.assets[address]);
                this.assets.Remove(address);
            }
        }

        public void UnloadAssetsAsync(Label label)
        {
            throw new NotImplementedException();
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