//// Copyright (C) OIEHOT - All Rights Reserved
//// Unauthorized copying of this file, via any medium is strictly prohibited
//// Proprietary and confidential
//// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 특정 컴포넌트 부착여부를 리턴한다.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <returns>컴포넌트 존재 유무</returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        /// <summary>
        /// 이 컴포넌트가 부착된 게임오브젝트에서 특정 컴포넌트를 찾는다. 없으면 예외를 발생시킨다.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetRequiredComponent<T>(this Component component) where T : Component
        {
            if (component.TryGetComponent<T>(out var neighborComponent))
            {
                return neighborComponent;
            }
            else
            {
                throw new MissingComponentException($"{component.gameObject.GetPath()} 게임오브젝트에 {nameof(T)} 컴포넌트가 부착되어있지 않음.");
            }
        }
    }
}