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
    public partial class SceneManager : MonoSingleton<SceneManager>
    {
        public enum Status
        {
            None,
            Initialized,
        }
        public Status status {get; private set;}
        Dictionary<Address, SceneInstance> _scenes;

        void Awake()
        {
            status = Status.None;
            _scenes = new Dictionary<Address, SceneInstance>();
        }

        /// <summary>
        /// 씬을 교체함. 로드된 모든 씬들은 언로드된다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> ChangeSceneAsync(Address address)
        {
            if (_scenes.ContainsKey(address))
            {
                throw new SceneManagerException($"씬 교체 실패: {address}는 이미 로드되어 있음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            handle = Addressables.LoadSceneAsync(address.value, LoadSceneMode.Single, true); // activateOnLoad:true
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 교체됨: {address}");
                    _scenes.Clear(); // 등재 씬 전부 삭제.
                    _scenes.Add(address, handle.Result); // 새 씬 등재
                }
                else
                {
                    throw new SceneManagerException($"씬 교체 실패: {address}");
                }
            };
            return handle;
        }

        /// <summary>
        /// 씬을 추가함. 기존에 열린 씬들은 닫히지 않는다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> AddSceneAsync(Address address)
        {
            if (_scenes.ContainsKey(address))
            {
                throw new SceneManagerException($"씬 추가 실패: {address}는 이미 로드되어 있음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            handle = Addressables.LoadSceneAsync(address.value, LoadSceneMode.Additive, activateOnLoad:true);
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 추가됨: {address}");
                    _scenes.Add(address, handle.Result); // SceneInstance 등재
                }
                else
                {
                    throw new SceneManagerException($"씬 추가 실패: {address}");
                }
            };
            return handle;
        }

        /// <summary>
        /// 씬을 제거한다. 기존에 열린 씬들은 건들지 않는다.
        /// autoReleaseHandle이 false면 Complete되어도 handle이 제거되지 않고 살아남게 된다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> RemoveSceneAsync(Address address, bool autoReleaseHandle=true)
        {
            if (_scenes.ContainsKey(address) == false)
            {
                throw new SceneManagerException($"씬 제거 실패: {address}가 로드되어있지 않음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            SceneInstance sceneInstance = _scenes[address];
            handle = Addressables.UnloadSceneAsync(sceneInstance, autoReleaseHandle);
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 제거: {address}");
                    _scenes.Remove(address);
                }
                else
                {
                    throw new SceneManagerException($"씬 제거 실패: {address}");
                }
            };
            return handle;
        }

        /// <summary>
        /// 씬을 활성화한다.
        /// </summary>
        /// <remarks>
        /// Instantiate된 개체가 지정되는 씬 활성화를 말한다.
        /// </remarks>
        public void ActivateScene(Address address)
        {
            if (_scenes.ContainsKey(address) == false)
            {
                throw new SceneManagerException($"씬 활성화 실패: {address}가 로드 되어있지 않음");
            }
            else
            {
                Log.Info($"현재 활성화 씬으로 지정: {address}");
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(_scenes[address].Scene);
                // _scenes[address].ActivateAsync(); // 로드 후 활성화
            }
        }

        // public void ReleaseHandle(AsyncOperationHandle handle)
        // {
        //     Addressables.Release(handle);
        // }
    }
}