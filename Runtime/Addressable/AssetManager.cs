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
    public enum AssetManagerStatus
    {
        None,
        Initialized,
    }

    public partial class AssetManager : MonoSingleton<AssetManager>
    {
        public AssetManagerStatus status {get; private set;}
        private Dictionary<Path, AssetInfo> assets;
        private Dictionary<Address, SceneInstance> scenes;
        private Dictionary<Path, string> pathToId;

        void Awake()
        {
            this.assets = new Dictionary<Path, AssetInfo>();
            this.scenes = new Dictionary<Address, SceneInstance>();
            this.pathToId = new Dictionary<Path, string>();
        }

        public AsyncOperationHandle<IResourceLocator> InitializeAsync()
        {
            AsyncOperationHandle<IResourceLocator> handle;
            handle = Addressables.InitializeAsync();
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.CacheCatalog(); // 카탈로그 캐싱
                    this.status = AssetManagerStatus.Initialized;
                    Log.Important("AssetManager: 초기화 성공");
                }
                else
                {
                    throw new Exception("AssetManager: 초기화 실패");
                }
            };
            return handle;
        }

        private void CacheCatalog()
        {
            ResourceLocationMap map = AssetManagerUtils.GetResourceLocationMap();
            if (map != null)
            {
                foreach (KeyValuePair<object, IList<IResourceLocation>> item in map.Locations)
                {
                    if (item.Key is System.String)
                    {
                        IList<IResourceLocation> locations = item.Value;
                        if (locations.Count >= 2)
                        {
                            // TODO: Label?
                            // TODO: 한 개 밖에 없는 레이블은 어떻게?
                            continue;
                        }
                        
                        Path path = new Path(locations[0].InternalId);
                        string id = (string)item.Key; // TODO: Check? GUID, Label, Address                        
                        if (this.pathToId.ContainsKey(path) == false)
                        {
                            // Log.Info($"CacheCatalog: {path} => {id}");
                            this.pathToId.Add(path, id);
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"CacheCatalog Failed: 카탈로그를 얻을 수 없음");
            }
        }

        /// <summary>비동기 에셋 로드/언로드 작업의 진행과정% 값을 Info 개체에 업데이트 함</summary>
        private IEnumerator UpdateAssetProgressCoroutine(AssetInfo assetInfo, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                assetInfo.percent = handle.PercentComplete;
                yield return null;
            }
            // TODO: Release?
                // Addressable.Release(handle);
        }

        public void ReleaseHandle(AsyncOperationHandle handle)
        {
            Addressables.Release(handle);
        }
    }
}