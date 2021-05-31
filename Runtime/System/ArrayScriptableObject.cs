// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;

namespace Rano
{
    public class ArrayScriptableObject<T> : ScriptableObject
    {
        public T[] items;
        public T this[int index]
        {
            get
            {
                return items[index];
            }
        }       
        public int Length { get => items.Length; }        
    }
}