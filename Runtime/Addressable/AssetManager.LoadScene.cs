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
        public SceneInfo LoadScene(AssetReference assetReference, LoadSceneMode loadSceneMode=LoadSceneMode.Additive)
        {
            Path path = AssetManagerUtils.GetPath(assetReference);
            return this.LoadScene(path, loadSceneMode);
        }

        public SceneInfo LoadScene(Address address, LoadSceneMode loadSceneMode=LoadSceneMode.Additive)
        {
            Path path = address.GetPath();
            return this.LoadScene(path, loadSceneMode);
        }

        /// <summary>어드레서블 씬을 로딩함</summary>
        /// TODO: path.IsScenePath() Check
        public SceneInfo LoadScene(Path path, LoadSceneMode loadSceneMode=LoadSceneMode.Additive)
        {
            SceneInfo sceneInfo;
            AsyncOperationHandle<SceneInstance> handle;

            if (!path.IsScenePath())
            {
                throw new Exception($"씬 로드 실패: {path} 는 씬이 아님");
            }

            if (this.scenes.ContainsKey(path))
            {
                sceneInfo = this.scenes[path];
                if (sceneInfo.status != SceneStatus.Unloaded)
                {
                    throw new Exception($"씬 로드 실패: {path} 가 이미 로드되어 있음");
                }
            }
            else
            {
                sceneInfo = new SceneInfo();
                this.scenes.Add(path, sceneInfo);
            }
            sceneInfo.id = this.pathToId[path]; // TODO: Label면 절대로 안된다.
            sceneInfo.path = path;
            sceneInfo.status = SceneStatus.Loading;
            sceneInfo.sceneInstance = null;
            sceneInfo.percent = 0;

            // 로딩 시작
            handle = Addressables.LoadSceneAsync(sceneInfo.id, loadSceneMode);
            handle.Completed += (handle) => {
                // 로딩 성공
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"씬 로드됨: {path}");
                    sceneInfo.status = SceneStatus.Loaded;
                    sceneInfo.sceneInstance = handle.Result; // handle.Result: SceneInstance
                    sceneInfo.percent = 100.0f;
                }
                // 로딩 실패
                else
                {
                    Debug.LogError($"씬 로드 실패: {path}");
                }
            };
            
            // 로딩 상태 업데이트
            StartCoroutine(this.UpdateSceneProgressCoroutine(sceneInfo, handle));
            
            return sceneInfo;
        }
    }
}