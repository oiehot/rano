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
    public enum QResourceManagerStatus
    {
        None,
        Initialized,
    }

    public partial class QResourceManager : MonoBehaviour
    {
        public QResourceManagerStatus status {get; private set;}
        private Queue<ResourceTask> queue;
        private Dictionary<Path, Resource> resources;
        private Dictionary<Path, string> pathToId;

        void Awake()
        {
            this.queue = new Queue<ResourceTask>();
            this.resources = new Dictionary<Path, Resource>();
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
                    this.status = QResourceManagerStatus.Initialized;
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

        void Start()
        {
            StartCoroutine(this.UpdateCoroutine());
        }

        IEnumerator UpdateCoroutine()
        {
            ResourceTask task = null;

            while (true)
            {
                if (task == null)
                {
                    if (this.queue.Count > 0)
                    {
                        task = this.queue.Dequeue();
                    }
                }

                if (task == null)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                if (task.status == ResourceTaskStatus.None)
                {
                    switch (task)
                    {
                        case LoadTask loadTask:
                            this.ProcessLoadTask(loadTask);
                            break;
                        case UnloadTask unloadTask:
                            this.ProcessUnloadTask(unloadTask);
                            break;
                    }
                }
                else if (task.status == ResourceTaskStatus.Done)
                {
                    task = null;
                }
                yield return new WaitForEndOfFrame();
                // yield return new WaitForSeconds(1.0f);
            }
        }

        IEnumerator UpdatePercentCoroutine(ResourceTask task, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                task.percent = handle.PercentComplete;
                yield return new WaitForEndOfFrame();
            }
            // TODO: Release?
            // Addressable.Release(handle);
        }        
    }
}