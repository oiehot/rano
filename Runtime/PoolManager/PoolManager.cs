// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    /// <summary>
    /// * PoolManager는 여러개의 Pool을 가지고 있다.
    /// * Hierarchy:
    ///     DontDestroyOnLoad
    ///     + RootPool
    ///         + PrefabA_Pool
    ///         + PrefabB_Pool
    ///             + PrefabB1
    ///             + PrefabB2
    /// </summary>
    /// <ref>
    /// * https://ansohxxn.github.io/unity%20lesson%202/ch10/
    /// </refs>
    [AddComponentMenu("Rano/Pool/PoolManager")]
    public class PoolManager : MonoBehaviour
    {
        Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        Transform _root;

        public void Initialize()
        {
            if (_root == null)
            {
                _root = new GameObject { name = "RootPool" }.transform;
                UnityEngine.Object.DontDestroyOnLoad(_root);
            }
        }

        public void CreatePool(GameObject original, int count=5)
        {
            Pool pool = new Pool();
            pool.Initialize(original, count);
            pool.root.parent = _root;
            _pools.Add(original.name, pool);
        }

        public void Push(Poolable poolable)
        {
            string name = poolable.gameObject.name;

            // 풀링되지 않는 오브젝트는 그냥 Destroy 한다.
            if (_pools.ContainsKey(name) == false)
            {
                GameObject.Destroy(poolable.gameObject);
                return;
            }
            _pools[name].Push(poolable);
        }

        public Poolable Pop(GameObject original, Transform parent = null)
        {
            if (_pools.ContainsKey(original.name) == false)
                CreatePool(original);
            
            return _pools[original.name].Pop(parent);
        }

        public GameObject GetOriginal(string name)
        {
            if (_pools.ContainsKey(name) == false)
                return null;
            return _pools[name].original;
        }

        public void Clear()
        {
            foreach (Transform child in _root)
                GameObject.Destroy(child.gameObject);
            _pools.Clear();
        }
    }
}