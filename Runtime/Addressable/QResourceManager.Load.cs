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
    public partial class QResourceManager : MonoBehaviour
    {
        public void Load(Path path)
        {
            ResourceLoadTask task = new ResourceLoadTask(path);
            this.queue.Enqueue(task);
        }

        public void Load(AssetReference assetReference)
        {
            Path path = ResourceManagerUtils.GetPath(assetReference);
            this.Load(path);
        }

        public void Load(Label label)
        {
            Path[] pathArray = label.GetPath();
            for (int i=0; i<pathArray.Length; i++)
            {
                this.Load(pathArray[i]);
            }
        }
        
        private void ProcessLoadTask(ResourceLoadTask task)
        {
            Path path;
            Resource resource;
            AsyncOperationHandle handle;

            path = task.path;

            if (this.resources.ContainsKey(path) == true)
            {
                resource = this.resources[path];
            }
            else
            {
                if (this.pathToId.ContainsKey(path) == false)
                {
                    throw new Exception($"{path}에 해당하는 어드레서블을 찾을 수 없음");
                }
                resource = new Resource();
                resource.path = path;
                resource.id = this.pathToId[path]; // Label, Address, AssetReference(GUID)
                this.resources.Add(path, resource);
            }
            task.status = ResourceTaskStatus.Working;
            handle = Addressables.LoadAssetAsync<object>(resource.id);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"리소스 로드 성공: {path}");
                    resource.asset = handle.Result;
                    task.percent = 1.0f;
                    task.status = ResourceTaskStatus.Done;
                }
                else
                {
                    task.status = ResourceTaskStatus.Failed;
                    Log.Warning($"리소스 로드 실패: {path}");
                }
            };
            StartCoroutine(UpdatePercentCoroutine(resource, handle));
        }
    }
}