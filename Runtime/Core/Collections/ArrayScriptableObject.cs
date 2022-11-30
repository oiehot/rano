using UnityEngine;

namespace Rano.Collections
{
    public class ArrayScriptableObject<T> : ScriptableObject
    {
        public T[] items;
        public T this[int index] => items[index];
        public int Length => items.Length;        
    }
}