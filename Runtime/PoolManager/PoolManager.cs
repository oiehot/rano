// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// * PoolManager는 여러개의 Pool을 가지고 있다.
    /// * Hierarchy:
    ///     DontDestroyOnLoad
    ///     + RootPool
    ///         + PrefabA_Pool
    ///         + PrefabB_Pool
    ///             + PrefabB1
    ///             + PrefabB2
    /// </remarks>
    /// <ref>
    /// * https://ansohxxn.github.io/unity%20lesson%202/ch10/
    /// </refs>
    [AddComponentMenu("Rano/Pool/PoolManager")]
    public class PoolManager : MonoBehaviour
    {
        Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        Transform _rootTransform;

        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize()
        {
            if (_rootTransform == null)
            {
                _rootTransform = new GameObject { name = "ObjectPools" }.transform;
                UnityEngine.Object.DontDestroyOnLoad(_rootTransform);
            }
        }

        /// <summary>
        /// 주어진 프리팹의 오브젝트 풀을 생성한다.
        /// </summary>
        public void CreatePool(GameObject prefab, int capacity=5)
        {
            string name = prefab.name;
            if (_pools.ContainsKey(name))
            {
                throw new Exception("이미 같은 프리팹 오브젝트 풀이 존재함.");
            }
            Pool pool = new Pool();
            pool.Initialize(prefab, capacity);
            pool.transform.parent = _rootTransform;
            _pools.Add(name, pool);
        }

        /// <summary>
        /// 풀에서 꺼냈던 게임오브젝트를 회수한다.
        /// </summary>
        /// <remark>
        /// 풀이 없는 게임오브젝트로 시도하면 예외가 발생한다.
        /// </remark>
        public void Push(Poolable poolable)
        {
            string name = poolable.gameObject.name;
            if (_pools.ContainsKey(name))
            {
                _pools[name].Push(poolable);
            }
            else
            {
                throw new Exception();
                // GameObject.Destroy(poolable.gameObject);
            }
        }

        /// <summary>
        /// 해당 프리팹의 오브젝트 풀에서 게임오브젝트를 꺼낸다.
        /// </summary>
        /// <remarks>
        /// 프리팹이 담길 오브젝트 풀이 없다면 새로 생성한다.
        /// parent의 기본값은 null로, 현재 활성화된 씬의 루트가 된다.
        /// </remarks>
        public Poolable Pop(GameObject prefab, Transform parent=null)
        {
            string name = prefab.name;
            if (_pools.ContainsKey(name) == false)
            {
                CreatePool(prefab);
            }
            return _pools[name].Pop(parent);
        }

        /// <summary>
        /// 프리팹 이름으로 오브젝트 풀을 얻는다.
        /// </summary>
        public Pool GetPool(string name)
        {
            if (_pools.ContainsKey(name) == true)
            {
                return _pools[name];
            }
            else
            {
                return null;
            }
        }        

        /// <summary>
        /// 프리팹으로 오브젝트 풀을 얻는다.
        /// </summary>
        public Pool GetPool(GameObject prefab)
        {
            return GetPool(prefab.name);
        }

        /// <summary>
        /// 오브젝트풀 이름으로 프리팹을 얻는다.
        /// </summary>
        public GameObject GetPrefab(string name)
        {
            if (_pools.ContainsKey(name) == true)
            {
                return _pools[name].prefab;
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            foreach (Transform child in _rootTransform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _pools.Clear();
        }
    }
}