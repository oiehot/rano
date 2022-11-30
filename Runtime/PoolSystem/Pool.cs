#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Rano.PoolSystem
{
    public class Pool
    {
        private readonly Stack<GameObject> _stack;
        private readonly GameObject _prefab;
        private readonly Transform _transform;

        public GameObject Prefab => _prefab;
        public Transform PoolTransform => _transform;
        
        public Pool(GameObject prefab, int capacity)
        {
            _stack = new Stack<GameObject>();
            _prefab = prefab;
            _transform = new GameObject().transform;
            _transform.name = $"{prefab.name}_Pool";
            for (int i=0; i<capacity; i++)
            {
                Push(Create());
            }
        }

        /// <summary>
        /// 프리팹으로 부터 게임오브젝트를 생성한다.
        /// </summary>
        GameObject Create()
        {
            GameObject gameObject;
            
            // TODO: 어드레서블인 경우 문제됨.
            gameObject = Object.Instantiate<GameObject>(_prefab);
            gameObject.name = _prefab.name;
            return gameObject;
        }

        /// <summary>
        /// 게임오브젝트를 재사용할 풀 스택에 넣는다.
        /// </summary>
        /// <remarks>
        /// 재사용 풀 스택에 넣으면 게임 오브젝트는 Deactive된다.
        /// </remarks>
        public void Push(GameObject gameObject)
        {
            gameObject.transform.parent = _transform;
            gameObject.SetActive(false);
            _stack.Push(gameObject);
        }

        /// <summary>
        /// 재사용 풀로부터 게임오브젝트 꺼내오기.
        /// </summary>
        public GameObject? Pop(Transform? parentTransform=null, bool createIfNotExists=true)
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
                    // TODO: 하나씩 생성하는게 아니라 초기 capacity의 단위의 곱으로 스택 사이즈를 증가시킬것.
                    gameObject = Create();
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
            if (parentTransform == null)
            {
                // DontDestroyOnLoad의 루트로 옮긴다.
                //
                // 루트로 옮기지 않으면 DontDestroyOnLoad > Pools > Prefab_Pool 이 Parent로 있는 것인데,
                // 다른 활성화 씬에는 동일한 Parent가 없으므로 문제가 된다. 따라서 루트로 일단 옮겨놓고
                // 다른 씬으로 이동해야 한다.
                gameObject.transform.parent = null;
                
                // 현재 활성화 씬의 루트로 옮긴다.
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
                    gameObject,
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                );
            }

            // 지정한 Transform 아래로 옮긴다. Transform은 활성화 씬에 있어야만 한다.
            gameObject.transform.parent = parentTransform;
            
            return gameObject;
        }
    }
}