using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.PoolSystem
{
    /// <summary>
    /// 이미 로드된 프리팹을 통해 오브젝트 풀을 만들고 생성(Pop)과 삭제(Push)를 관리한다.
    /// </summary>
    /// <remarks>
    /// PoolManager는 여러개의 Pool을 가지고 있다.
    /// 다음과 같은 계층구조로 풀과 게임오브젝트들이 생성된다:
    /// 
    /// Pools
    ///   PrefabA_Pool
    ///     PrefabA
    ///   PrefabB_Pool
    ///     PrefabB
    ///     PrefabB
    ///   
    /// </remarks>
    /// <ref>
    /// https://ansohxxn.github.io/unity%20lesson%202/ch10/
    /// </refs>
    public sealed class PoolManager : ManagerComponent
    {
        Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        Transform _rootTransform;

        protected override void Awake()
        {
            base.Awake();
            if (_rootTransform == null)
            {
                _rootTransform = new GameObject { name = "Pools" }.transform;
                UnityEngine.Object.DontDestroyOnLoad(_rootTransform);
            }
        }

        /// <summary>
        /// 주어진 프리팹의 오브젝트 풀을 생성한다.
        /// </summary>
        public void CreatePool(GameObject prefab, int capacity=10)
        {
            string name = prefab.name;
            if (_pools.ContainsKey(name))
            {
                throw new Exception("프리팹 오브젝트 풀이 이미 존재함.");
            }
            Log.Info($"풀({name}) 생성.");
            Pool pool = new Pool();
            pool.Initialize(prefab, capacity);
            pool.transform.parent = _rootTransform;
            _pools.Add(name, pool);
        }

        // TODO: 어드레서블 에셋을 통해 풀 만들기. 이미 로드되어 있어야 한다.
        // public void CreatePool(AssetReferenceGameObject assetReference, int capacity=5)
        // {
        //     if (assetReference.isDone)
        //     {
        //         CreatePool(assetReference.Asset, capacity);
        //     }
        //     else
        //     {
        //         throw new Exception($"로드되어 있지 않아 풀을 만들 수 없습니다: {assetReference}");
        //     }
        // }

        /// <summary>
        /// 풀에서 오브젝트를 꺼낸다.
        /// </summary>
        /// <remarks>
        /// 프리팹이 담길 오브젝트 풀이 없다면 새로 생성한다.
        /// parent의 기본값은 null로, 현재 활성화된 씬의 루트가 된다.
        /// </remarks>
        public GameObject Pop(GameObject prefab, Transform parent=null)
        {
            string name = prefab.name;
            if (_pools.ContainsKey(name) == false)
            {
                CreatePool(prefab);
            }
            return _pools[name].Pop(parent);
        }
        /// <summary>
        /// 이름을 통해 풀에서 오브젝트를 꺼낸다.
        /// </summary>
        /// <remarks>
        /// 풀이 없다면 null을 리턴한다.
        /// parent의 기본값은 null로, 현재 활성화된 씬의 루트가 된다.
        /// </remarks>
        public GameObject Pop(string name, Transform parent=null)
        {
            if (_pools.ContainsKey(name) == true)
            {
                return _pools[name].Pop(parent);
            }
            return null;
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
        /// 풀에 오브젝트를 넣는다.
        /// </summary>
        /// <remark>
        /// 풀이 없는 게임오브젝트로 시도하면 예외가 발생한다.
        /// </remark>    
        public void Push(GameObject gameObject)
        {
            string name = gameObject.name;
            if (_pools.ContainsKey(name))
            {
                _pools[name].Push(gameObject);
            }
            else
            {
                throw new Exception("풀이 없어서 Push할 수 없음.");
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