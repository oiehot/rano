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
    public sealed class AddressableSceneManager : ManagerComponent
    {
        Dictionary<object, SceneInstance> _scenes;

        protected override void Awake()
        {
            base.Awake();
            _scenes = new Dictionary<object, SceneInstance>();
        }

        /// <summary>
        /// 씬을 교체함. 로드된 모든 씬들은 언로드된다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> ChangeSceneAsync(object key)
        {
            if (_scenes.ContainsKey(key))
            {
                throw new AddressableSceneManagerException($"씬 교체 실패: {key}는 이미 로드되어 있음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single, activateOnLoad:true);
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 교체됨: {key}");
                    _scenes.Clear(); // 등재 씬 전부 삭제.
                    _scenes.Add(key, handle.Result); // 새 씬 등재
                }
                else
                {
                    throw new AddressableSceneManagerException($"씬 교체 실패: {key}");
                }
            };
            return handle;
        }

        /// <summary>
        /// 씬을 추가함. 기존에 열린 씬들은 닫히지 않는다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> AddSceneAsync(object key)
        {
            if (_scenes.ContainsKey(key))
            {
                throw new AddressableSceneManagerException($"씬 추가 실패: {key}는 이미 로드되어 있음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive, activateOnLoad:true);
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 추가됨: {key}");
                    _scenes.Add(key, handle.Result); // SceneInstance 등재
                }
                else
                {
                    throw new AddressableSceneManagerException($"씬 추가 실패: {key}");
                }
            };
            return handle;
        }

        /// <summary>
        /// 씬을 제거한다. 기존에 열린 씬들은 건들지 않는다.
        /// autoReleaseHandle이 false면 Complete되어도 handle이 제거되지 않고 살아남게 된다.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> RemoveSceneAsync(object key, bool autoReleaseHandle=true)
        {
            if (_scenes.ContainsKey(key) == false)
            {
                throw new AddressableSceneManagerException($"씬 제거 실패: {key}가 로드되어있지 않음");
            }

            AsyncOperationHandle<SceneInstance> handle;
            SceneInstance sceneInstance = _scenes[key];
            handle = Addressables.UnloadSceneAsync(sceneInstance, autoReleaseHandle);
            handle.Completed += (handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Log.Info($"씬 제거: {key}");
                    _scenes.Remove(key);
                }
                else
                {
                    throw new AddressableSceneManagerException($"씬 제거 실패: {key}");
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
        public void ActivateScene(object key)
        {
            if (_scenes.ContainsKey(key) == false)
            {
                throw new AddressableSceneManagerException($"씬 활성화 실패: {key}가 로드 되어있지 않음");
            }
            else
            {
                Log.Info($"현재 활성화 씬으로 지정: {key}");
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(_scenes[key].Scene);
                // _scenes[address].ActivateAsync(); // 로드 후 활성화
            }
        }

        #if false
        // WARNING: Add/Remove로 씬을 추가하지 않은 경우 등재되지 않아서 찾지를 못함.
        /// <summary>
        /// 현재 활성화되어 있는 씬의 어드레스를 얻는다.
        /// </summary>
        public Address GetActiveScene()
        {
            Scene activeScene;
            activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            foreach (KeyValuePair<Address,SceneInstance> item in _scenes)
            {
                // Log.Info($"{item.Value.Scene.path} == {activeScene.path} ?");
                if (item.Value.Scene == activeScene)
                {
                    return item.Key;
                }
            }
            throw new SceneManagerException("현재 활성화된 씬이 캐싱되어있지 않아 리턴할 수 없음. SceneManager에서 Add, Remove되지 않은 Scene은 저장되지 않아 못찾을 수 있습니다.");
        }
        #endif
    }
}