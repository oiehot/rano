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
        Initializing,
        Initialized,
        InitializeFailed,
        Downloading,
        Downloaded,
        DownloadFailed,
    }

    public class DownloadTask
    {
        public string id;
        public long size;
        public long downloadedSize
        {
            get
            {
                return (long)(size * percent);
            }
        }
        public float percent;
        public DownloadStatus status;

        public DownloadTask(string key)
        {
            id = key;
            size = 0;
            percent = 0.0f;
            status = DownloadStatus.None;
        }

        public bool IsDownloaded()
        {
            if (this.status == DownloadStatus.Downloaded) return true;
            else return false;
        }

        public override string ToString()
        {
            return $"DownloadTask(id:{id}, status:{status}, size:{size}, percent:{percent})";
        }        
    }
}
#endif