using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rano
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// 주어진 Bounds 값과 합친 Bounds로 업데이트 한다.
        /// </summary>
        public static void Combine(this ref Bounds bounds, Bounds item)
        {
            bounds.Encapsulate(item);
        }

        /// <summary>
        /// 주어진 Bounds 값들과 합친 Bounds로 업데이트 한다.
        /// </summary>
        public static void Combine(this ref Bounds bounds, IEnumerable<Bounds> items)
        {
            foreach (Bounds item in items)
            {
                bounds.Encapsulate(item);
            }
        }
        
        /// <summary>
        /// 중앙점을 기준으로 스케일을 변경한다.
        /// </summary>
        public static void Multiply(this ref Bounds bounds, float scale)
        {
            bounds.extents *= scale;
        }

        /// <summary>
        /// Width/Height 비례를 얻는다.
        /// </summary>
        public static float GetAspect(this Bounds bounds)
        {
            return bounds.size.x / bounds.size.y;
        }
    }
}