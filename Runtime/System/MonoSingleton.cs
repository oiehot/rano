// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static bool _shuttingDown = false;
        static object _lock = new object();
    
        #if UNITY_EDITOR
            static T _instance;
            public static T Instance
            {
                get
                {
                    if (_shuttingDown)
                    {
                        Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                        return null;
                    }

                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = (T)FindObjectOfType(typeof(T));
                            if (_instance == null)
                            {
                                GameObject gameObject = new GameObject();
                                _instance = gameObject.AddComponent<T>();
                                gameObject.name = typeof(T).ToString();
                                Debug.LogWarning($"싱글톤 클래스가 없는 상태에서 액세스되어 새로 생성했습니다.");
                                Debug.LogWarning($"이 경고가 보이면 발생하지 않도록 수정해주십시요. 퍼포먼스가 저하됩니다.");
                                // DontDestroyOnLoad(gameObject);
                            }
                        }
                        return _instance;
                    }
                }
            }
        #else
            public static T Instance {get; private set;}
        #endif
        
        void Awake()
        {
            #if (UNITY_EDITOR == false)
            Instance = this;
            #endif
            DontDestroyOnLoad(gameObject);
        }

        void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        void OnDestroy()
        {
            _shuttingDown = true;
        }
    }
}