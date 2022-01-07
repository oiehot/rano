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
    public class AddressablePool
    {
        public int Available => _stack.Count;
        public int Capacity { get; private set; }

        private Stack<GameObject> _stack;
        public string AssetName { get; private set; }
        public AssetReferenceGameObject Reference { get; private set;}
        public Transform PoolTransform {get; private set; }

        public AddressablePool()
        {
            _stack = new Stack<GameObject>();
        }

        private void ExtendCapacity(int size)
        {
            int newCapacity = Capacity + size;
            Log.Info($"풀({AssetName}) 확장 ({Capacity} => {newCapacity})");
            for (int i = 0; i < size; i++)
            {
                var asyncOp = Addressables.InstantiateAsync(Reference);
                var gameObject = asyncOp.Result;
                gameObject.name = AssetName;
                Push(gameObject);
            }
            Capacity = newCapacity;
        }

        /// <summary>
        /// 풀을 초기화한다.
        /// </summary>
        public void Initialize(AssetReferenceGameObject reference, int capacity)
        {
            // 프리팹의 이름 캐싱
            AssetName = reference.Asset.name;

            // 에셋레퍼런스 캐싱
            Reference = reference;

            // 풀 트랜스폼을 만들고 Capacity에 따라 인스턴스 생성.
            Log.Info($"풀({AssetName}) 생성 시작 (x{capacity})");
            PoolTransform = new GameObject().transform;
            PoolTransform.name = $"{AssetName}_Pool";
            ExtendCapacity(capacity);
            Log.Info($"풀({AssetName}) 생성 완료");
        }

        /// <summary>
        /// 캐싱된 모든 게임오브젝트들을 Release 하고
        /// 풀 그룹용 게임오브젝트, 캐싱하던 AssetReferenceObject, 스택 인스턴스 등을 모두 제거한다.
        /// </summary>
        public void Release() // TODO: IDispose, Dipose?
        {
            while (_stack.Count > 0)
            {
                var gameObject = _stack.Pop();
                Addressables.ReleaseInstance(gameObject);
            }
            GameObject.Destroy(PoolTransform.gameObject);
            AssetName = null;
            PoolTransform = null;
            Reference = null;
        }

#if false
        /// <summary>
        /// AssetRefernceGameObject를 사용해서 게임오브젝트를 생성한다.
        /// </summary>
        private GameObject Instantiate()
        {
            var handle = Addressables.InstantiateAsync(Reference);
            var gameObject = handle.WaitForCompletion();
            gameObject = handle.Result;
            gameObject.name = AssetName;
            gameObject.SetActive(false);
            return gameObject;
        }
#endif

        /// <summary>
        /// 게임오브젝트를 재사용할 풀 스택에 넣는다.
        /// </summary>
        /// <remarks>
        /// 재사용 풀 스택에 넣으면 게임 오브젝트는 Deactive된다.
        /// </remarks>
        public void Push(GameObject gameObject)
        {
            gameObject.transform.SetParent(PoolTransform);
            gameObject.SetActive(false);
            _stack.Push(gameObject);
        }

        /// <summary>
        /// 재사용 풀로부터 게임오브젝트 꺼내오기.
        /// </summary>
        public GameObject Pop(Transform parent, bool createIfNotExists=true)
        {
            GameObject gameObject;

            // 재사용 가능한 게임오브젝트가 있다면 꺼내고 없으면 새로 생성한다.
            if (_stack.Count > 0)
            {
                gameObject = _stack.Pop();
            }
            else
            {
                if (createIfNotExists)
                {
                    ExtendCapacity(Capacity); // Capacity 두 배로.
                    gameObject = _stack.Pop();
                }
                else
                {
                    return null;
                }
            }

            // 게임오브젝트를 활성화 한다.
            gameObject.SetActive(true);

            // 꺼낼 게임오브젝트가 들어갈 Parent Transform이 지정되어있지 않다면,
            // 현재 활성화 씬 루트 Transform 에 넣는다.
            // DontDestroyOnLoad가 아닌 현재 활성화 씬으로 옮긴다.
            if (parent == null)
            {
                // DontDestroyOnLoad의 루트로 옮긴다.
                //
                // 루트로 옮기지 않으면 DontDestroyOnLoad > Pools > Prefab_Pool 이 Parent로 있는 것인데,
                // 다른 활성화 씬에는 동일한 Parent가 없으므로 문제가 된다. 따라서 루트로 일단 옮겨놓고
                // 다른 씬으로 이동해야 한다.
                //gameObject.transform.parent = null;
                gameObject.transform.SetParent(null);

                // 현재 활성화 씬의 루트로 옮긴다.
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
                    gameObject,
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                );
            }
            else
            {
                // 지정한 Transform 아래로 옮긴다. Transform은 활성화 씬에 있어야만 한다.
                // TODO: Remove => gameObject.transform.parent = parent;
                gameObject.transform.SetParent(parent);
            }
            
            return gameObject;
        }
    }
}