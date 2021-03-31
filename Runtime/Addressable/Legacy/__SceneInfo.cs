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
    public enum SceneStatus
    {
        Loading,
        Loaded,
        Unloading,
        Unloaded,
    }
    
    public class SceneInfo
    {
        public string id; // Address or AssetReference(GUID)
        public Path path;
        public SceneInstance? sceneInstance;
        public SceneStatus status;
        public float percent;

        public bool IsLoaded()
        {
            if (status == SceneStatus.Loaded) return true;
            else return false;
        }
        
        public override string ToString()
        {
            return $"Scene: path:{path}, status:{status}, percent:{percent}";
        }
    }
}

#endif