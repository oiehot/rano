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
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Rano.Addressable
{
    public enum ResourceTaskStatus
    {
        None,
        Initalized,
        Working,
        Failed,
        Done
    }

    public class ResourceTask
    {
        public ResourceTaskStatus status;
        public float percent;

        public ResourceTask()
        {
            this.status = ResourceTaskStatus.None;
            this.percent = 0.0f;
        }
    }

    public class LoadTask : ResourceTask
    {
        public Path path;

        public LoadTask(Path path)
        {
            this.path = path;
        }
    }

    public class UnloadTask : ResourceTask
    {
        public Path path;

        public UnloadTask(Path path)
        {
            this.path = path;
        }        
    }
}

#endif