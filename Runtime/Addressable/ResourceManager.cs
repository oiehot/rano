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
    public enum ResourceManagerState
    {
        None,
        Initialized,
    }

    public partial class ResourceManager : MonoBehaviour
    {
        public ResourceManagerState state {get; private set;}
        private Dictionary<Path, object> assets;
        private Dictionary<Path, string> pathToId;
        
        void Awake()
        {
            this.state = ResourceManagerState.None;
            this.assets = new Dictionary<Path, object>();
            this.pathToId = new Dictionary<Path, string>();
        }

        public void InitializeAsync()
        {
            AsyncOperationHandle<IResourceLocator> handle;
            handle = Addressables.InitializeAsync();
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.CacheCatalog(); // 카탈로그 캐싱
                    this.state = ResourceManagerState.Initialized;
                    Log.Important("ResourceManager: 초기화 성공");
                }
                else
                {
                    throw new Exception("ResourceManager: 초기화 실패");
                }
            };
        }
        
        private void CacheCatalog()
        {
            ResourceLocationMap map = ResourceManagerUtils.GetResourceLocationMap();
            if (map != null)
            {
                foreach (KeyValuePair<object, IList<IResourceLocation>> item in map.Locations)
                {
                    if (item.Key is System.String)
                    {
                        IList<IResourceLocation> locations = item.Value;
                        if (locations.Count >= 2) continue; // Label => Skip
                        
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
    }
}