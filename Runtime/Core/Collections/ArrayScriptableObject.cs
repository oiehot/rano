using System;
using UnityEngine;

namespace Rano.Collections
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