using System;
using UnityEngine;
using Rano;
using UnityEngine.AddressableAssets;

namespace Rano
{
    public abstract class AddressableSingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
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
                        string typeName = typeof(T).Name;
                        Log.Info($"Loading ScriptableObject from Addressable ({typeName})");
                        var handle = Addressables.LoadAssetAsync<T>(typeName);
                        var result = handle.WaitForCompletion();
                        if (result == null)
                        {
                            throw new Exception($"어드레서블된 ScriptableObject를 찾을 수 없음 ({typeName}, {typeName})");
                        }
                        _instance = result;
                    }
                    return _instance;
                }
            }
        }

        public void EmptyMethod()
        {
        }
    }
}