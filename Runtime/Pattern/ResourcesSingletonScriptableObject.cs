using System;
using UnityEngine;
using Rano;

namespace Rano
{
    public abstract class ResourcesSingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        Log.Info($"Loading from Resources ({typeof(T).Name})");
                        _instance = Resources.Load(typeof(T).Name) as T;
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Instance를 생성하기 위한 빈 메소드.
        /// </summary>
        public void EmptyMethod()
        {
        }
    }
}