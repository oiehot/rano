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

    public partial class AssetDownloader : MonoBehaviour
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
            // TODO: 코루틴이 실행중이면 중지한다.

            // 현재 작업 리스트를 비운다. (히스토리 리스트는 비우지 않는다)
            this.currentTasks.Clear();
        }

        public void Run()
        {
            StartCoroutine(this.DownloaderCoroutine());
        }

        public bool IsDownloaded()
        {
            if (this.status == AssetDownloaderStatus.Downloaded) return true;
            else return false;
        }

        private IEnumerator DownloaderCoroutine()
        {
            int totalCount = this.currentTasks.Count;

            // 초기화: 용량 체크
            Log.Info("에셋다운로더: 초기화중...");
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
            }

            // 초기화가 완료될 때까지 대기
            Log.Info("에셋다운로더: 초기화 완료 대기중...");
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
            
            // 초기화 완료
            Log.Info("에셋다운로더: 초기화 완료");
            this.status = AssetDownloaderStatus.Initialized;

            // 다운로드 시작
            Log.Info("에셋다운로더: 다운로드중...");
            this.status = AssetDownloaderStatus.Downloading;

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
            }

            // 다운로드 완료 대기
            Log.Info("에셋다운로더: 다운로드 완료 대기중...");
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

        public DownloadTask QueueDownload(string key)
        {
            DownloadTask task;
            if (this.tasks.ContainsKey(key) == true)
            {
                task = this.tasks[key];
                if (task.status == DownloadStatus.Downloaded) return null;
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

        // public void ClearCache()
        // {
        //     Log.Info($"캐시 카운트: {Caching.cacheCount}");
        //     bool result;
        //     result = Caching.ClearCache();
        //     Log.Info($"캐시 클리어 결과: {result}");
        // }
    }
}