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
        private Dictionary<Path, Resource> resources;
        private Queue<ResourceTask> queue;

        void Awake()
        {
            this.resources = new Dictionary<Path, Resource>();
            this.queue = new Queue<ResourceTask>();
        }

        void Start()
        {
            StartCoroutine(this.UpdateCoroutine);
        }

        IEnumerator UpdateCoroutine()
        {
            ResourceTask task;

            while (true)
            {
                if (task == null) task = this.queue.Dequeue();
                if (task == null) yield return new WaitForEndOfFrame();

                if (task.status == ResourceTaskStatus.None)
                {
                    switch (task)
                    {
                        case ResourceLoadTask task:
                            this.ProcessLoadTask(task);
                            break;
                        case ResourceUnloadTask task:
                            this.ProcessUnloadTask(task);
                            break;
                    }
                }
                else if (task.status == ResoucreTaskStatus.Done)
                {
                    task = null;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator UpdatePercentCoroutine(ResourceTask task, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                task.percent = handle.Percent;
                yield return new WaitForEndOfFrame();
            }
            Addressable.Release(handle);
        }        
    }
}