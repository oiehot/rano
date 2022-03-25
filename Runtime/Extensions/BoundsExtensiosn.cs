// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rano
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// �־��� Bounds ���� ��ģ Bounds�� ������Ʈ �Ѵ�.
        /// </summary>
        public static void Combine(this ref Bounds bounds, Bounds item)
        {
            bounds.Encapsulate(item);
        }

        /// <summary>
        /// �־��� Bounds ����� ��ģ Bounds�� ������Ʈ �Ѵ�.
        /// </summary>
        public static void Combine(this ref Bounds bounds, IEnumerable<Bounds> items)
        {
            foreach (Bounds item in items)
            {
                bounds.Encapsulate(item);
            }
        }
    }
}