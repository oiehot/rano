// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 게임오브젝트에 부착된 컴포넌트를 돌려준다. 없으면 새로 생성해서 돌려준다.
        /// </summary>
        /// <typeparam name="T">돌려줄 컴포넌트 타입.</typeparam>
        /// <param name="gameObject">해당 컴포넌트가 부착된 게임오브젝트.</param>
        /// <returns>컴포넌트</returns>
        static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            else
                return gameObject.AddComponent<T>();
        }
    }
}