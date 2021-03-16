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
            UnloadTask task = new UnloadTask(path);
            this.queue.Enqueue(task);
        }
        
        private void ProcessUnloadTask(UnloadTask task)
        {
            throw new NotImplementedException();
        }
    }
}