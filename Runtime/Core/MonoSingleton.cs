// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    /// <summary>
    /// 안전한 MonoBehaviour 싱글톤
    /// </summary>
    /// <remarks>
    /// 준비된 게임오브젝트와 컴포넌트가 없으면 새로 만든다.
    /// 한번 Unload 후 다시 Load 되면 셧다운 플래그로 인해서 에러가 발생한다.
    /// </remarks>    
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance;
        static bool _shuttingDown = false;
        static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
#if UNITY_EDITOR
                    Log.Warning($"싱글톤 {typeof(T)}가 제거된 상태에서 사용되었습니다.");
#endif
                    return null;
                }

                lock (_lock)
                {
                    // UnityEngine.Object의 비교연산자는 오버로딩되어 성능이 떨어진다.
                    // 네이티브 객체를 비교하지 않고 유니티 객체(래핑객체)만 비교하도록 한다.
                    //if (_instance == null)
                    if (object.ReferenceEquals(_instance, null))
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (_instance == null)
                        {
                            GameObject gameObject = new GameObject();
                            _instance = gameObject.AddComponent<T>();
                            gameObject.name = typeof(T).ToString();
#if UNITY_EDITOR
                            Log.Info($"싱글톤 {typeof(T)}가 자동으로 생성되었습니다.");
#endif
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                    return _instance;
                }
            }
        }

        //void OnApplicationQuit()
        //{
        //    _shuttingDown = true;
        //}

        void OnDestroy()
        {
            _shuttingDown = true;
        }
    }
}