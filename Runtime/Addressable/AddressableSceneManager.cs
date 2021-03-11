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
    public struct Address
    {
        public string value;
        
        public Address(string address)
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
    
    public enum SceneStatus
    {
        Loading,
        Loaded,
        Unloading,
        Unloaded,
    }
    
    public class SceneInfo
    {
        public Address address;
        public SceneStatus status;
        public SceneInstance? sceneInstance;
        public float percent;
        
        public override string ToString()
        {
            return $"{address} (status:{status}, percent:{percent})";
        }
    }
    
    public class AddressableSceneManager : MonoBehaviour
    {
        private Dictionary<Address, SceneInfo> scenes;

        void Awake()
        {
            this.scenes = new Dictionary<Address, SceneInfo>();
        }
                
        /// <summary>어드레서블 씬을 로딩함</summary>
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(Address address, LoadSceneMode loadSceneMode)
        {
            SceneInfo sceneInfo;
            AsyncOperationHandle<SceneInstance> handle;

            if (this.scenes.ContainsKey(address))
            {
                sceneInfo = this.scenes[address];
                if (sceneInfo.status != SceneStatus.Unloaded)
                {
                    throw new Exception($"Load Scene Failed: {address} already loaded");
                }
            }
            else
            {
                sceneInfo = new SceneInfo();
                this.scenes.Add(address, sceneInfo);
            }
            sceneInfo.address = address;
            sceneInfo.status = SceneStatus.Loading;
            sceneInfo.sceneInstance = null;
            sceneInfo.percent = 0;

            // 로딩 시작
            handle = Addressables.LoadSceneAsync(address.value, loadSceneMode);
            handle.Completed += (handle) => {
                // 로딩 성공
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Scene Loaded: {address.value}");
                    sceneInfo.status = SceneStatus.Loaded;
                    sceneInfo.sceneInstance = handle.Result; // handle.Result: SceneInstance
                    sceneInfo.percent = 100.0f;
                }
                // 로딩 실패
                else
                {
                    Debug.LogError($"Load Scene Failed: {address}");
                }
            };
            
            // 로딩 상태 업데이트
            StartCoroutine(this.UpdateProgress(handle, sceneInfo));
            
            return handle;
        }

        /// <summary>어드레서블 씬을 언로드함</summary>
        public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(Address address)
        {
            SceneInfo sceneInfo;
            AsyncOperationHandle<SceneInstance> handle;

            if (!this.scenes.ContainsKey(address)) 
            {
                throw new Exception($"Unload Scene Failed: {address} was not loaded");
            }
            
            sceneInfo = this.scenes[address];

            if (sceneInfo.status != SceneStatus.Loaded)
            {
                throw new Exception($"Unload Scene Failed: {address} was not loaded");
            }

            if (!sceneInfo.sceneInstance.HasValue)
            {
                throw new Exception($"Unload Scene Failed: {address} sceneInstance not found");
            }

            // 언로딩 시작
            sceneInfo.status = SceneStatus.Unloading;
            handle = Addressables.UnloadSceneAsync(sceneInfo.sceneInstance.Value);
            handle.Completed += (handle) => {
                // 언로딩 완료
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Scene Unloaded: {address}");
                    sceneInfo.status = SceneStatus.Unloaded;
                    sceneInfo.sceneInstance = null;
                    sceneInfo.percent = 0.0f;
                }
                // 언로딩 실패
                else
                {
                    Debug.LogError($"Unload Scene Failed: {address}");
                }
            };

            // 언로딩 상태 업데이트
            StartCoroutine(this.UpdateProgress(handle, sceneInfo));
            
            return handle;
        }
        
        private IEnumerator UpdateProgress(AsyncOperationHandle<SceneInstance> handle, SceneInfo sceneInfo)
        {
            while (!handle.IsDone)
            {
                sceneInfo.percent = handle.PercentComplete;
                yield return null;
            }
        }

        /// <summary>씬 상태 정보를 얻는다</summary>
        public SceneInfo GetSceneInfo(Address address)
        {   
            if (this.scenes.ContainsKey(address))
            {
                return this.scenes[address];
            }
            return null;
        }

        public void DebugLogScenes()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("----");
            foreach (KeyValuePair<Address,SceneInfo> item in this.scenes)
            {
                SceneInfo sceneInfo = item.Value;
                Debug.Log($"{item.Value}");
            }
            #endif
        }
    }
}