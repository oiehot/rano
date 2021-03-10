// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

// Reference: https://github.com/Unity-Technologies/Addressables-Sample/tree/master/Basic/Scene%20Loading

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
    public struct AddressableAddress
    {
        public string value;
        
        public AddressableAddress(string address)
        {
            value = address;
        }

        public override string ToString()
        {
            return $"{value.ToString()}";
        }        
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
    
    public enum AddressableSceneStatus
    {
        Loading,
        Loaded,
        Unloading,
        Unloaded,
    }
    
    public class AddressableSceneInfo
    {
        public AddressableAddress address;
        public AddressableSceneStatus status;
        public SceneInstance sceneInstance;
        public float percent;
        
        public override string ToString()
        {
            return $"{address} (status:{status}, percent:{percent})";
        }
    }
    
    public class AddressableSceneManager : MonoBehaviour
    {
        private Dictionary<AddressableAddress, AddressableSceneInfo> scenes;

        void Awake()
        {
            this.scenes = new Dictionary<AddressableAddress, AddressableSceneInfo>();
        }
        
        public AddressableSceneInfo GetInfo(string address)
        {
            AddressableAddress addressableAddress;
            addressableAddress = new AddressableAddress(address);
            
            if (this.scenes.ContainsKey(addressableAddress))
            {
                return this.scenes[addressableAddress];
            }
            return null;
        }
        
        /// <summary>어드레서블 씬을 로딩함</summary>
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(AddressableAddress address, LoadSceneMode loadSceneMode)
        {
            if (this.scenes.ContainsKey(address))
            {
                throw new Exception($"Load Scene Failed: {address} already loaded");
            }
            
            AddressableSceneInfo info = new AddressableSceneInfo();
            info.address = address;
            info.status = AddressableSceneStatus.Loading;
            info.percent = 0;
            this.scenes.Add(address, info);
            
            AsyncOperationHandle<SceneInstance> handle;
            handle = Addressables.LoadSceneAsync(address.value, loadSceneMode);
            
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    info.sceneInstance = handle.Result; // handle.Result: SceneInstance
                    info.status = AddressableSceneStatus.Loaded;
                    info.percent = 100;
                    Debug.Log($"Scene Loaded: {address.value}");
                }
                else
                {
                    Debug.LogError($"Load Scene Failed: {address}");
                }
            };
            
            StartCoroutine(this.UpdateProgress(handle, info));
            
            return handle;
        }

        /// <summary>어드레서블 씬을 언로드함</summary>
        public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AddressableAddress address)
        {
            if (!this.scenes.ContainsKey(address)) 
            {
                throw new Exception($"Unload Scene Failed: {address} was not loaded");
            }
            
            AddressableSceneInfo info;
            AsyncOperationHandle<SceneInstance> handle;
            
            info = this.scenes[address];
            info.status = AddressableSceneStatus.Unloading;
            handle = Addressables.UnloadSceneAsync(info.sceneInstance);
            
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Scene Unloaded: {address}");
                    info.status = AddressableSceneStatus.Unloaded;
                    this.scenes.Remove(address);
                }
                else
                {
                    Debug.LogError($"Unload Scene Failed: {address}");
                }
            };
            
            StartCoroutine(this.UpdateProgress(handle, info));
            
            return handle;
        }
        
        private IEnumerator UpdateProgress(AsyncOperationHandle<SceneInstance> handle, AddressableSceneInfo info)
        {
            while (!handle.IsDone)
            {
                Debug.Log($"Load Scene {handle.PercentComplete}%");
                info.percent = handle.PercentComplete;
                yield return null;
            }
        }
    }
}