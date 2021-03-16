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
    public partial class ResourceManager : MonoBehaviour
    {
        /// <summary>Address를 통해 캐싱된 에셋을 Instantiate 한다</summary>
        public AsyncOperationHandle<GameObject> InstantiateAsync(Address address)
        {
            return Addressables.InstantiateAsync(address.value);
        }

        /// <summary>AssetReference를 통해 캐싱된 에셋을 Instantiate 한다</summary>
        public AsyncOperationHandle<GameObject> InstantiateAsync(AssetReference assetReference)
        {
            return Addressables.InstantiateAsync(assetReference);
        }

        /// <summary>Path로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
        private GameObject InstantiateSync(Path path)
        {
            GameObject prefab = this.Get<GameObject>(path);
            return UnityEngine.Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        /// <summary>Address로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
        public GameObject InstantiateSync(Address address)
        {
            Path path = address.GetPath();
            return this.InstantiateSync(path);
        }

        /// <summary>AssetReference로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
        public GameObject InstantiateSync(AssetReference assetReference)
        {
            Path path = ResourceManagerUtils.GetPath(assetReference);
            return this.InstantiateSync(path);
        }

        /// <summary>게임 오브젝트를 릴리즈한다</summary>
        public void ReleaseInstance(GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}