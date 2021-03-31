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
    public enum AssetDownloaderStatus
    {
        None,
        Initalizing,
        Initialized,
        InitializeFailed,
        Downloading,
        Downloaded,
        DownloadFailed
    }

    public partial class AssetDownloader : MonoSingleton<AssetDownloader>
    {
        private Dictionary<string, DownloadTask> tasks;
        private List<DownloadTask> currentTasks;
        public AssetDownloaderStatus status {get; private set;}

        public string cachePath
        {
            get
            {
                return Application.persistentDataPath;
            }
        }

        public long size
        {
            get
            {
                int totalCount = this.currentTasks.Count;
                long result = 0;
                for (int i=0; i<totalCount; i++)
                {
                    result += this.currentTasks[i].size;
                }
                return result;
            }
        }

        public long downloadedSize
        {
            get
            {
                int totalCount = this.currentTasks.Count;
                long result = 0;
                for (int i=0; i<totalCount; i++)
                {
                    result += this.currentTasks[i].downloadedSize;
                }
                return result;
            }
        }

        public float percent
        {
            get
            {
                int totalCount = this.currentTasks.Count;
                float totalPercent = 0.0f;
                for (int i=0; i<totalCount; i++)
                {
                    totalPercent += this.currentTasks[i].percent;
                }
                if (totalCount > 0) return totalPercent / totalCount;
                else return 0.0f;
            }
        }

        public override string ToString()
        {
            return $"AssetDownloader(status:{this.status}, percent:{this.percent}, size:{this.downloadedSize}/{this.size})";
        }

        void Awake()
        {
            this.status = AssetDownloaderStatus.None;
            this.tasks = new Dictionary<string, DownloadTask>();
            this.currentTasks = new List<DownloadTask>();
        }

        public void Clear()
        {
            switch (this.status)
            {
                case AssetDownloaderStatus.Initalizing:
                case AssetDownloaderStatus.Downloading:
                    throw new Exception("에셋다운로더: 초기화나 다운로드가 완료된 상태에서만 작업리스트를 비울 수 있습니다");
                case AssetDownloaderStatus.None:
                case AssetDownloaderStatus.Initialized:
                case AssetDownloaderStatus.Downloaded:
                case AssetDownloaderStatus.InitializeFailed:
                case AssetDownloaderStatus.DownloadFailed:
                    Log.Info("에셋다운로더: 작업 리스트 리셋");
                    this.status = AssetDownloaderStatus.None;
                    this.currentTasks.Clear();
                    break;
            }
        }

        public void Run()
        {
            StartCoroutine(this.RunCoroutine());
        }

        public bool IsDownloaded()
        {
            if (this.status == AssetDownloaderStatus.Downloaded) return true;
            else return false;
        }

        private IEnumerator RunCoroutine()
        {
            yield return this.Initialize();
            yield return this.WaitInitialize();
            yield return this.Download();
            yield return this.WaitDownload();
        }

        private IEnumerator Initialize()
        {
            Log.Info("에셋다운로더: 초기화중...");
            this.status = AssetDownloaderStatus.Initalizing;
            int totalCount = this.currentTasks.Count;
            for (int i=0; i<totalCount; i++)
            {
                DownloadTask task = this.currentTasks[i];
                task.status = DownloadStatus.Initializing;
                Addressables.GetDownloadSizeAsync(task.id)
                    .Completed += (AsyncOperationHandle<long> handle) => {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            task.status = DownloadStatus.Initialized;
                            task.size = handle.Result; // 다운로드 사이즈 얻기. 0이면 이미 캐싱된 것이다.
                        }
                        else
                        {
                            Log.Warning($"다운로드작업 초기화 실패: {task.id}");
                            task.status = DownloadStatus.InitializeFailed;
                        }
                        // TODO: Addressables.Release(handle);
                    };
                yield return null;
            }
        }

        private IEnumerator WaitInitialize()
        {
            Log.Info("에셋다운로더: 초기화 완료 대기중...");
            int totalCount = this.currentTasks.Count;            
            while (true)
            {
                int initializedCount = 0;
                for (int i=0; i<totalCount; i++)
                {
                    DownloadTask task = this.currentTasks[i];
                    if (task.status == DownloadStatus.InitializeFailed)
                    {
                        this.status = AssetDownloaderStatus.InitializeFailed;
                        Log.Warning($"다운로더 초기화 실패");
                        yield break;; // 코루틴 종료
                    }

                    if (task.status == DownloadStatus.Initialized)
                    {
                        initializedCount++;
                    }
                }
                if (initializedCount >= totalCount) break; // 초기화 종료
                else yield return null;
            }

            Log.Info("에셋다운로더: 초기화 완료");
            this.status = AssetDownloaderStatus.Initialized;            
        }
        
        private IEnumerator Download()
        {
            if (this.status != AssetDownloaderStatus.Initialized)
            {
                Log.Warning("에셋다운로더: 초기화되어 있지않아 다운로드를 시작하지 않습니다");
                yield break;
            }

            Log.Info("에셋다운로더: 다운로드중...");
            this.status = AssetDownloaderStatus.Downloading;            

            int totalCount = this.currentTasks.Count;
            for (int i=0; i<totalCount; i++)
            {
                AsyncOperationHandle handle;
                DownloadTask task = this.currentTasks[i];

                // 이미 캐싱되어 있어서 다운로드 작업을 수행하지 않음.
                if (task.size <= 0)
                {
                    Log.Info($"다운로드 생략: {task.id}는 이미 캐싱되어 있음");
                    task.percent = 1.0f;
                    task.status = DownloadStatus.Downloaded;
                    continue;
                }

                task.status = DownloadStatus.Downloading;
                handle = Addressables.DownloadDependenciesAsync(task.id);
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
                        task.status = DownloadStatus.DownloadFailed;
                    }
                    // TODO: Addressables.Release(handle);
                };
                StartCoroutine(this.UpdateDownloadTaskProgress(task, handle));
                yield return null;
            }
        }

        private IEnumerator WaitDownload()
        {
            Log.Info("에셋다운로더: 다운로드 완료 대기중...");

            int totalCount = this.currentTasks.Count;
            
            while (true)
            {
                int downloadedCount = 0;
                for (int i=0; i<totalCount; i++)
                {
                    DownloadTask task = this.currentTasks[i];
                    if (task.status == DownloadStatus.DownloadFailed)
                    {
                        this.status = AssetDownloaderStatus.DownloadFailed;
                        Log.Warning($"에셋다운로더: 다운로드 실패");
                        yield break;; // 코루틴 종료
                    }

                    if (task.status == DownloadStatus.Downloaded)
                    {
                        downloadedCount++;
                    }
                }
                if (downloadedCount >= totalCount) break;
                else yield return null;
            }

            // 다운로드 완료
            Log.Info("에셋다운로더: 다운로드 완료");
            this.status = AssetDownloaderStatus.Downloaded;  
        }

        private IEnumerator UpdateDownloadTaskProgress(DownloadTask info, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                info.percent = handle.PercentComplete;
                yield return new WaitForEndOfFrame();
            }
        }        

        private DownloadTask QueueDownload(string key)
        {
            DownloadTask task;
            if (this.tasks.ContainsKey(key) == true)
            {
                task = this.tasks[key];
                if (task.status == DownloadStatus.Downloaded)
                {
                    return null;
                }
            }
            else
            {
                task = new DownloadTask(key);
                this.tasks.Add(key, task);
            }
            this.currentTasks.Add(task);
            return task;
        }

        public DownloadTask QueueDownload(Label label)
        {
            return this.QueueDownload(label.value);
        }  

        private void ClearCacheAsync(string key)
        {
            Addressables.ClearDependencyCacheAsync(key);
        }

        public void ClearCacheAsync(Label label)
        {
            this.ClearCacheAsync(label.value);
        }
    }
}