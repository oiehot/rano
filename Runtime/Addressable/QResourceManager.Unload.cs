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
        public void Unload(Path path)
        {
            ResourceUnloadTask task = new ResourceUnloadTask(path);
            this.taskQueue.Enqueue(task);
        }
        
        private void ProcessUnloadTask(ResourceUnloadTask task)
        {
            throw new NotImplementedException();
        }
    }
}