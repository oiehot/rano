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
        /// Hierarchy상 게임오브젝트의 경로를 돌려준다.
        /// </summary>
        /// <param name="gameObject"></param>
        /// </example>
        public static string GetPath(this GameObject gameObject)
        {
            string path = "/" + gameObject.name;
            while (gameObject.transform.parent != null)
            {
                gameObject = gameObject.transform.parent.gameObject;
                path = "/" + gameObject.name + path;
            }
            return path;
        }

        /// <summary>
        /// 게임오브젝트에 부착된 컴포넌트를 돌려준다. 없으면 새로 생성해서 돌려준다.
        /// </summary>
        /// <typeparam name="T">돌려줄 컴포넌트 타입.</typeparam>
        /// <param name="gameObject">해당 컴포넌트가 부착된 게임오브젝트.</param>
        /// <returns>컴포넌트</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            else
                return gameObject.AddComponent<T>();
        }
    }
}