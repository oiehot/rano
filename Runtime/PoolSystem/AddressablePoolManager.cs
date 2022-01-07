// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rano;

namespace Rano.PoolSystem
{
    /// <summary>
    /// 어드레서블 에셋레퍼런스를 통해 오브젝트 풀을 만들고 생성(Pop)과 삭제(Push)를 관리한다.
    /// </summary>
    /// <remarks>
    /// PoolManager는 여러개의 AdressablePool을 가지고 있다.
    /// 다음과 같은 계층구조로 풀과 게임오브젝트들이 생성된다:
    /// 
    /// Pools
    ///   AssetRefernceGameObjectA_AddressablePool
    ///     AssetReferenceGameObjectA
    ///   AssetRefernceGameObjectB_AddressablePool
    ///     AssetReferenceGameObjectB
    ///     AssetReferenceGameObjectB
    ///   
    /// </remarks>
    public sealed class AddressablePoolManager : Singleton<AddressablePoolManager>
    {
        private Dictionary<string, AddressablePool> _pools;
        private Transform _rootTransform;

        public AddressablePoolManager()
        {
            Log.Sys($"{typeof(AddressablePoolManager).ToString()}: Construct", caller: false);
            _pools = new Dictionary<string, AddressablePool>();
            if (_rootTransform == null)
            {
                _rootTransform = new GameObject { name = "AddressablePools" }.transform;
                UnityEngine.Object.DontDestroyOnLoad(_rootTransform);
            }
        }

        public async Task CreatePoolAsync(AssetReferenceGameObject reference, int capacity)
        {
            // 에셋이 로드되어 있지 않다면 로드
            if (reference.Asset == null)
            {
                var handle = reference.LoadAssetAsync();
                await handle.Task;
            }

            // 프리팹 에셋의 이름으로 풀이 이미 있는지 체크
            string assetName = reference.Asset.name;
            if (_pools.ContainsKey(assetName))
            {
                throw new Exception($"오브젝트 풀이 이미 존재함 ({assetName})");
            }

            // 풀을 생성
            AddressablePool pool = new AddressablePool();
            pool.Initialize(reference, capacity);
            pool.PoolTransform.parent = _rootTransform;
            _pools.Add(assetName, pool);
        }

        public void ReleasePool(string assetName)
        {
            AddressablePool pool;
            if (_pools.TryGetValue(assetName, out pool))
            {
                pool.Release();
                _pools.Remove(assetName);
            }
        }

        public void ReleasePool(AssetReferenceGameObject reference)
        {
            ReleasePool(reference.Asset.name);
        }

        public void Clear()
        {
            foreach (var kv in _pools)
            {
                AddressablePool pool = kv.Value;
                pool.Release();
            }
            _pools.Clear();
        }

        public AddressablePool GetPool(string assetName)
        {
            AddressablePool pool;
            if (_pools.TryGetValue(assetName, out pool))
            {
                return pool;
            }
            else
            {
                return null;
            }
        }

        public AddressablePool GetPool(AssetReferenceGameObject reference)
        {
            return GetPool(reference.Asset.name);
        }

        public GameObject Pop(string assetName, Transform parent = null)
        {
            AddressablePool pool;
            GameObject gameObject;

            if (_pools.TryGetValue(assetName, out pool))
            {
                gameObject = pool.Pop(parent);
            }
            else
            {
                return null;
            }
            return gameObject;
        }

        public GameObject Pop(AssetReferenceGameObject reference, Transform parent=null)
        {
            return Pop(reference.Asset.name, parent);
        }

        /// <summary>
        /// 풀에 오브젝트를 넣는다.
        /// </summary>
        /// <remark>
        /// 풀이 없는 게임오브젝트로 시도하면 예외가 발생한다.
        /// 게임오브젝트의 이름이 AssetReferenceGameObject.AssetGUID 와 동일해야함.
        /// </remark>
        public void Push(GameObject gameObject)
        {
            AddressablePool pool;
            string assetName = gameObject.name;
            if (_pools.TryGetValue(assetName, out pool) == true)
            {
                pool.Push(gameObject);
            }
            else
            {
                throw new Exception("풀이 없어서 Push할 수 없음.");
            }
        }
    }
}