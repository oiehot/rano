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
        /// <returns></returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }
    }
}