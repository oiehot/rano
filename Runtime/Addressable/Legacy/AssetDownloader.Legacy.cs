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
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Rano.Addressable
{
    public enum DownloadStatus
    {
        None,
        Downloading,
        Downloaded,
        Failed,
    }

    public class DownloadTask
    {
        public string id;
        public float percent;
        public DownloadStatus status;

        public bool IsDownloaded()
        {
            if (this.status == DownloadStatus.Downloaded) return true;
            else return false;
        }

        public override string ToString()
        {
            return $"DownloadTask(id:{id}, status:{status}, percent:{percent})";
        }        
    }

    public partial class AssetDownloader : MonoBehaviour
    {
        public DownloadStatus status {get; private set;}
        public float percent;

        private Dictionary<string, DownloadTask> tasks;

        void Awake()
        {
            this.status = DownloadStatus.None;
            this.percent = 0.0f;
            this.tasks = new Dictionary<string, DownloadTask>();
        }
        
        /// <summary>관련된 모든 에셋번들을 다운로드함</summary>
        private DownloadTask DownloadAsync(string key)
        {
            DownloadTask task;
            AsyncOperationHandle handle;

            if (this.tasks.ContainsKey(key) == true)
            {
                task = this.tasks[key];
            }
            else
            {
                task = new DownloadTask();
                task.id = key;
                task.percent = 0.0f;
                task.status = DownloadStatus.None;
                this.tasks.Add(key, task);
            }

            if (task.status == DownloadStatus.Downloaded)
            {
                // throw new Exception($"이미 다운로드 되어있음: {task}");
                return task;
            }

            task.status = DownloadStatus.Downloading;
            handle = Addressables.DownloadDependenciesAsync(key);
            handle.Completed += (AsyncOperationHandle handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"다운로드 성공: {task}");
                    task.percent = 1.0f;
                    task.status = DownloadStatus.Downloaded;
                }
                else
                {
                    Log.Warning($"다운로드 실패: {task}");
                    task.status = DownloadStatus.Failed;
                }
                // TODO: Release?
                // Addressables.Release(handle);
            };

            StartCoroutine(this.UpdateDownloadProgressCoroutine(task, handle));

            return task;
        }

        public DownloadTask DownloadAsync(Label label)
        {
            return this.DownloadAsync(label.value);
        }
        
        /// <summary>
        /// 다운로드 받아두었던 캐시를 지운다.
        /// </summray>
        private void ClearCacheAsync(string key)
        {
            Addressables.ClearDependencyCacheAsync(key);
        }

        public void ClearCacheAsync(Label label)
        {
            this.ClearCacheAsync(label.value);
        }

        /// <summary>
        /// 다운로드 사이즈 얻기. 0이면 이미 캐싱된 것이다.
        /// </summary>
        private void GetDownloadSizeAsync(string key)
        {
            Addressables.GetDownloadSizeAsync(key)
                .Completed += (AsyncOperationHandle<long> handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"Download Size: {key} => {handle.Result}");
                    }
                    else
                    {
                        Log.Warning($"Get Download Size Failed: {key}");
                    }
                    // TODO: Release?
                    // Addressables.Release(handle);
                };
        }

        private IEnumerator UpdateDownloadProgressCoroutine(DownloadTask info, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                info.percent = handle.PercentComplete;
                yield return new WaitForEndOfFrame();
            }            
        }

        public bool IsFinished()
        {
            throw new NotImplementedException();
        }
    }
}

#endif