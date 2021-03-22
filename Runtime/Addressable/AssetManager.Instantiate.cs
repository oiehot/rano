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
    public partial class AssetManager : MonoBehaviour
    {
        #region Async

            private AsyncOperationHandle<GameObject> _InstantiateAsync(object key, string name)
            {
                AsyncOperationHandle<GameObject> handle;
                handle = Addressables.InstantiateAsync(key);
                if (name != null)
                {
                    handle.Completed += (handle) => {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject gameObject = handle.Result;
                            gameObject.name = name;
                        }
                    };
                }
                return handle;
            }

            public AsyncOperationHandle<GameObject> InstantiateAsync(Address address, string name)
            {
                return this._InstantiateAsync(address.value, name);
            }

            public AsyncOperationHandle<GameObject> InstantiateAsync(AssetReference assetReference, string name)
            {
                return this._InstantiateAsync(assetReference, name);
            }

            public void DestroyAsync(GameObject gameObject)
            {
                Addressables.ReleaseInstance(gameObject);
            }

            public void DestroyAsync(string name)
            {
                GameObject gameObject;
                gameObject = GameObject.Find(name);
                if (gameObject != null)
                {
                    this.DestroyAsync(gameObject);
                }
            }

        #endregion

        #region Sync

            /// <summary>Path로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
            private GameObject Instantiate(Path path)
            {
                GameObject prefab = this.GetAsset<GameObject>(path);
                return UnityEngine.Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            }

            /// <summary>Address로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
            public GameObject Instantiate(Address address)
            {
                Path path = address.GetPath();
                return this.Instantiate(path);
            }

            /// <summary>AssetReference로 캐싱된 프리팹을 얻고 즉시 인스턴스화함</summary>
            public GameObject Instantiate(AssetReference assetReference)
            {
                Path path = AssetManagerUtils.GetPath(assetReference);
                return this.Instantiate(path);
            }

            public void Destroy(GameObject gameObject)
            {
                UnityEngine.Object.Destroy(gameObject);
            }

            // public void Destroy(string name)
            // {
            //     GameObject gameObject;
            //     gameObject = GameObject.Find(name);
            //     this.Destroy(gameObject);
            // }

        #endregion
    }
}