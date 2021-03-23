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
    public partial class AssetManager : MonoBehaviour
    {
        public AsyncOperationHandle<SceneInstance> UnloadScene(AssetReference assetReference)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.UnloadScene(path);
        }

        public AsyncOperationHandle<SceneInstance> UnloadScene(Address address)
        {
            Path path = address.GetPath();
            return this.UnloadScene(path);
        }

        /// <summary>어드레서블 씬을 언로드함</summary>
        public AsyncOperationHandle<SceneInstance> UnloadScene(Path path)
        {
            SceneInfo sceneInfo;
            AsyncOperationHandle<SceneInstance> handle;

            if (!path.IsScenePath())
            {
                throw new Exception($"씬 언로드 실패: {path} 는 씬이 아님");
            }

            if (!this.scenes.ContainsKey(path)) 
            {
                throw new Exception($"씬 언로드 실패: {path} 가 로드되어 있지 않음");
            }
            
            sceneInfo = this.scenes[path];

            if (sceneInfo.status != SceneStatus.Loaded)
            {
                throw new Exception($"씬 언로드 실패: {path} 가 로드되어 있지 않음");
            }

            if (!sceneInfo.sceneInstance.HasValue)
            {
                throw new Exception($"씬 언로드 실패: {path} 씬 인스턴스가 없음");
            }

            // 언로딩 시작
            sceneInfo.status = SceneStatus.Unloading;
            handle = Addressables.UnloadSceneAsync(sceneInfo.sceneInstance.Value);
            handle.Completed += (handle) => {
                // 언로딩 완료
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"씬 언로드: {path}");
                    sceneInfo.status = SceneStatus.Unloaded;
                    sceneInfo.sceneInstance = null;
                    sceneInfo.percent = 0.0f;
                }
                // 언로딩 실패
                else
                {
                    Debug.LogError($"씬 언로드 실패: {path}");
                }
            };

            // 언로딩 상태 업데이트
            StartCoroutine(this.UpdateSceneProgressCoroutine(sceneInfo, handle));
            
            return handle;
        }        
    }
}