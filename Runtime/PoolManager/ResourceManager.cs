// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    [AddComponentMenu("Rano/Pool/ResourceManager")]
    public class ResourceManager : MonoBehaviour
    {
        /// <summary>
        /// 프리팹 로드 또한 풀에 있으면 풀에서 가져온다.
        /// 풀에 없는 프리팹이라면 로드한다.
        /// </summary>
        /// <remarks>
        /// 오브젝트 Pool에서 Instantiate 를 줄이려 하는 것처럼
        /// </remarks>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            if (typeof(T) == typeof(GameObject))
            {
                string name = path;
                int index = name.LastIndexOf('/'); // * '/' 뒤의 이름을 추출.
                if (index >= 0)
                    name = name.Substring(index + 1); // 프리팹의 이름
                GameObject go = GameManager.Pool.GetOriginal(name);
                if (go != null)
                    return go as T;
            }

            // 풀에서 못 찾았다면 힘들게 로딩
            return Resources.Load<T>(path); // UnityEngine의 Resource
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// 해당 프리팹에 Poolable이 있다는 것은 풀링으로 관리되는 오브젝트라를 뜻.
        /// 이 경우에는 Instantiate 하지 않고 풀에서 생성된 게임오브젝트를 재사용한다.
        /// 해당 프리팹에 Poolable이 없다면, Object.Instantiate 한다.
        /// </remarks>
        public GameObject Instantiate(string path, Transform parent=null)
        {
            GameObject original = Load<GameObject>($"Prefabs/{path}");
            if (original == null)
            {
                Log.Info($"Failed to load prefab: {path}");
                return null;
            }
            if (original.GetComponent<Poolable>() != null)
            {
                return GameManager.Pool.Pop(original, parent).gameObject;
            }
            GameObject go = Object.Instantiate(original, parent);
            go.name = original.name;
            return go;
        }

        /// <summary>
        /// 게임 오브젝트를 제거하거나 재활용하기 위해 끈다.
        /// </summary>
        /// <remarks>
        /// Poolable이 있다면 풀링을 통해 끈다.
        /// Poolable이 없다면 Object.Destroy 통해 제거한다.
        /// </remarks>
        /// <example>
        /// <code>
        /// go = ResourceManager.Instantiate("Characters/SantaPrefab")
        /// ResourceManager.Destroy(go);
        /// </code>
        /// </example>
        public void Destroy(GameObject go)
        {
            if (go == null) return;
            
            if (go.TryGetComponent<Poolable>(var poolable))
            {
                GameManager.Pool.Push(poolable);
                return;
            }
            else
            {
                Object.Destroy(go);
            }
        }
    }
}