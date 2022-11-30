#nullable enable

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
    /// <seealso href="https://ansohxxn.github.io/unity%20lesson%202/ch10">참고 자료</seealso>
    public sealed class PoolManager : ManagerComponent
    {
        #nullable disable
        private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        private Transform _rootTransform;
        #nullable enable

        protected override void Awake()
        {
            base.Awake();
            
            GameObject poolsGameObject = new GameObject();
            poolsGameObject.name = "Pools";
            _rootTransform = poolsGameObject.transform;
            DontDestroyOnLoad(_rootTransform);
        }

        /// <summary>
        /// 주어진 프리팹의 오브젝트 풀을 생성한다.
        /// </summary>
        public void CreatePool(GameObject prefab, int capacity=10)
        {
            string prefabName = prefab.name;
            if (_pools.ContainsKey(prefabName))
            {
                throw new Exception("프리팹 오브젝트 풀이 이미 존재함.");
            }
            Log.Info($"풀({prefabName}) 생성.");
            Pool pool = new Pool(prefab, capacity);
            pool.PoolTransform.parent = _rootTransform;
            _pools.Add(prefabName, pool);
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
        public GameObject? Pop(GameObject prefab, Transform? parentTransform=null)
        {
            string prefabName = prefab.name;
            if (_pools.ContainsKey(prefabName) == false)
            {
                CreatePool(prefab);
            }
            return _pools[prefabName].Pop(parentTransform);
        }
        /// <summary>
        /// 이름을 통해 풀에서 오브젝트를 꺼낸다.
        /// </summary>
        /// <remarks>
        /// 풀이 없다면 null을 리턴한다.
        /// parent의 기본값은 null로, 현재 활성화된 씬의 루트가 된다.
        /// </remarks>
        public GameObject? Pop(string prefabName, Transform? parentTransform=null)
        {
            if (_pools.ContainsKey(prefabName) == false) return null;
            return _pools[prefabName].Pop(parentTransform);
        }

        /// <summary>
        /// 프리팹 이름으로 오브젝트 풀을 얻는다.
        /// </summary>
        public Pool? GetPoolByName(string prefabName)
        {
            if (_pools.ContainsKey(prefabName) == false) return null;
            return _pools[prefabName];
        }

        /// <summary>
        /// 풀에 오브젝트를 넣는다.
        /// </summary>
        /// <remark>
        /// 풀이 없는 게임오브젝트로 시도하면 예외가 발생한다.
        /// </remark>    
        public void Push(GameObject go)
        {
            string prefabName = go.name;
            if (_pools.ContainsKey(prefabName) == false)
            {
                throw new Exception("풀이 없어서 Push할 수 없음.");
            }
            _pools[prefabName].Push(go);
        }

        /// <summary>
        /// 프리팹으로 오브젝트 풀을 얻는다.
        /// </summary>
        public Pool? GetPoolByPrefab(GameObject prefab)
        {
            return GetPoolByName(prefab.name);
        }

        /// <summary>
        /// 오브젝트풀 이름으로 프리팹을 얻는다.
        /// </summary>
        public GameObject? GetPrefabByName(string prefabName)
        {
            if (_pools.ContainsKey(prefabName))
            {
                return _pools[prefabName].Prefab;
            }
            return null;
        }

        public void Clear()
        {
            foreach (Transform child in _rootTransform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            _pools.Clear();
        }
    }
}