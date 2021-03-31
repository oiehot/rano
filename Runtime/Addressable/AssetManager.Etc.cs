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
    public partial class AssetManager : MonoSingleton<AssetManager>
    {
        #region Etc

        /// <summary>
        /// Ref-Count가 0지만 언로드되지 않은 에셋은 강제로 언로드한다.
        /// Ref-Count가 0더라도, 에셋번들이 완전히 Unload 되지 않은 경우 Asset은 Unload되지 않는다.
        /// 주의: 잠깐 멈출 수 있음.
        /// </summary>
        public void FreeUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        #endregion
    }
}