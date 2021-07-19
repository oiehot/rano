// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    internal class Pool
    {
        public GameObject prefab {get; private set;}
        public Transform transform {get; private set;}
        Stack<Poolable> _stack = new Stack<Poolable>();

        /// <summary>
        /// 재사용 풀을 초기화한다.
        /// </summary>
        public void Initialize(GameObject prefab, int capacity)
        {
            this.prefab = prefab;
            transform = new GameObject().transform;
            transform.name = $"{prefab.name}_Pool";
            for (int i=0; i<capacity; i++)
            {
                Push(Create());
            }
        }

        /// <summary>
        /// 프리팹으로 부터 게임오브젝트를 생성한다.
        /// </summary>
        Poolable Create()
        {
            GameObject go = UnityEngine.Object.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            return go.GetOrAddComponent<Poolable>();
        }

        /// <summary>
        /// 게임오브젝트를 재사용할 풀 스택에 넣는다.
        /// </summary>
        /// <remarks>
        /// 재사용 풀 스택에 넣으면 게임 오브젝트는 Deactive된다.
        /// </remarks>
        public void Push(Poolable poolable)
        {
            if (poolable == null) return;
            poolable.transform.parent = transform;
            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;
            _stack.Push(poolable);
        }

        /// <summary>
        /// 재사용 풀로부터 게임오브젝트 꺼내오기
        /// </summary>
        public Poolable Pop(Transform parent, bool createIfNotExists=true)
        {
            Poolable poolable;

            // 재사용 가능한 게임오브젝트가 있다면 꺼내고
            // 없으면 새로 생성한다.
            if (_stack.Count > 0)
                poolable = _stack.Pop();
            else
                if (createIfNotExists)
                    poolable = Create();
                else
                    return null;
            
            // 게임오브젝트를 활성화 한다.
            poolable.gameObject.SetActive(true);

            // 꺼낼 게임오브젝트가 들어갈 Parent Transform이 지정되어있지 않다면,
            // 현재 활성화 씬 루트 Transform 에 넣는다.
            // DontDestroyOnLoad 해제 용도: 
            //   한번 DontDestroyOnLoad가 되면 transform.parent = null 이 되어도
            //   DontDestroyOnLoad 안의 루트로만 빠져 나가는 문제가 있음.
            //   꼼수 해결방법으로, @Scene의 tranform을 부모로 설정해서 DontDestroyOnLoad를 빠져나가게 만들고
            //   다시 한번 parent를 설정하는 것.
            if (parent == null)
            {
                poolable.transform.parent = SceneManager.CurrentScene.transform;
            }
            poolable.transform.parent = parent;
            
            // 마무리
            poolable.isUsing = true;

            return poolable;
        }
    }
}