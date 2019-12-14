// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace Rano.Encoding
{
    [System.Serializable]
    public class ArrayWrapper<T>
    {
        public T[] items;
    }

    public static class Json
    {
        
        public static T FromJson<T>(string jsonText)
        {
            T obj = JsonUtility.FromJson<T>(jsonText);
            return obj;
        }
        public static T[] FromArrayJson<T>(string jsonText)
        {
            ArrayWrapper<T> a = JsonUtility.FromJson< ArrayWrapper<T> >(jsonText);
            return a.items;
        }
        
        public static string ToJson<T>(T obj, bool prettyPrint=true)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }
        
        public static string ToJson<T>(T[] array, bool prettyPrint=true)
        {
            ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
    }
}