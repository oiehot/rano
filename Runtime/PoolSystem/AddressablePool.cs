#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rano.PoolSystem
{
    public class AddressablePool
    {
        private readonly Stack<GameObject> _stack;
        private int _capacity;
        
        private AssetReferenceGameObject? _assetReference;
        private string? _assetName;
        
        private GameObject? _poolGameObject;
        private Transform? _poolTransform;

        public Transform? PoolTransform => _poolTransform;

        public AddressablePool(AssetReferenceGameObject assetReference, int capacity)
        {
            _stack = new Stack<GameObject>();
            
            _assetReference = assetReference;
            _assetName = assetReference.Asset.name;

            // 풀 트랜스폼을 만들고 Capacity에 따라 인스턴스 생성.
            Log.Info($"풀({_assetName}) 생성 시작 (x{capacity})");
            _poolGameObject = new GameObject();
            _poolTransform = _poolGameObject.transform;
            _poolTransform.name = $"{_assetName}_Pool";
            
            ExtendCapacity(capacity);
        }

        private void ExtendCapacity(int size)
        {
            int newCapacity = _capacity + size;
            
            Log.Info($"풀({_assetName}) 확장 ({_capacity} => {newCapacity})");
            for (int i = 0; i < size; i++)
            {
                var asyncOp = Addressables.InstantiateAsync(_assetReference);
                var gameObject = asyncOp.Result;
                gameObject.name = _assetName;
                Push(gameObject);
            }
            _capacity = newCapacity;
        }

        /// <summary>
        /// 캐싱된 모든 게임오브젝트들을 Release 하고
        /// 풀 그룹용 게임오브젝트, 캐싱하던 AssetReferenceObject, 스택 인스턴스 등을 모두 제거한다.
        /// </summary>
        public void Release()
        {
            Log.Info($"풀({_assetName}) 삭제 중...");
            
            // 스택을 비우고 어드레서블로 Instantiate된 인스턴스들을 릴리즈한다.
            while (_stack.Count > 0)
            {
                var gameObject = _stack.Pop();
                if (gameObject != null)
                {
                    Addressables.ReleaseInstance(gameObject);
                }
            }

            if (_poolGameObject != null)
            {
                Object.Destroy(_poolGameObject);
                _poolGameObject = null;
            }
            _assetName = null;
            _poolTransform = null;
            _assetReference = null;
        }

        /// <summary>
        /// 게임오브젝트를 재사용할 풀 스택에 넣는다.
        /// </summary>
        /// <remarks>
        /// 재사용 풀 스택에 넣으면 게임 오브젝트는 Deactive된다.
        /// </remarks>
        public void Push(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(_poolTransform);
            _stack.Push(gameObject);
        }

        /// <summary>
        /// 재사용 풀로부터 게임오브젝트 꺼내오기.
        /// </summary>
        public GameObject? Pop(Transform? parentTransform=null, bool createIfNotExists=true)
        {
            GameObject? gameObject;

            // 재사용 가능한 게임오브젝트가 있다면 꺼내고 없으면 새로 생성한다.
            if (_stack.Count > 0)
            {
                gameObject = _stack.Pop();
                if (gameObject == null)
                {
                    Log.Warning("스택에서 Pop된 게임오브젝트가 null임");
                    return null;
                }
            }
            else
            {
                if (createIfNotExists)
                {
                    ExtendCapacity(_capacity); // Capacity 두 배로.
                    gameObject = _stack.Pop();
                    if (gameObject == null)
                    {
                        Log.Warning("스택이 확장되었으나 Pop된 게임오브젝트가 null임");
                        return null;
                    }
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
            if (!parentTransform)
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
                gameObject.transform.SetParent(parentTransform);
            }
            
            return gameObject;
        }
    }
}