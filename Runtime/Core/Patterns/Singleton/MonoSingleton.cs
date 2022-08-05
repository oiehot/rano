#if false

using UnityEngine;

namespace Rano.Pattern
{
    /// <summary>
    /// MonoBehaviour 싱글톤
    /// </summary>
    /// <remarks>
    /// 준비된 게임오브젝트와 컴포넌트가 없으면 새로 만든다.
    /// 한번 Unload 후 다시 Load 되면 셧다운 플래그로 인해서 에러가 발생한다.
    /// </remarks>    
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isAppQuitting = false;
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                if (_isAppQuitting)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Log.Info($"모노싱글톤 인스턴스 {typeof(T)}가 종료되었음에도 사용되었습니다.");
#endif
                    return null;
                }

                lock (Lock)
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            Log.Info($"싱글톤 {typeof(T)}가 자동으로 생성되었습니다.");
#endif
                        }
                    }
                    return _instance;
                }
            }
        }

// #if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
//         private static void ReloadDomain()
//         {
//             Log.Info($"For UnityReloadDomain, Reset Instance");
//             _isAppQuitting = false;
//             _instance = null;
//         }
// #endif

        protected virtual void Awake()
        {
            if (_instance && _instance != this)
            {
                Log.Error($"{typeof(T)} 가 이미 존재합니다!");
                // Destroy(gameObject);
                return;
            }
            Log.Sys($"{typeof(T).ToString()}: Awake", caller: false);
            //gameObject.name = typeof(T).ToString();
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnEnable()
        {
            Log.Sys($"{typeof(T).ToString()}: OnEnable", caller: false);
        }

        /// <summary>
        /// 앱을 종료하면 실행된다.
        /// OnApplicationQuit > OnDisable > OnDestroy 순서로 실행됨.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            //Log.Sys($"{typeof(T).ToString()}: OnApplicationQuit", caller: false);
            // OnApplicationQuit => OnDisable => OnDestroy
            _isAppQuitting = true;
        }

        protected virtual void OnDisable()
        {
            Log.Sys($"{typeof(T).ToString()}: OnDisable", caller: false);
        }


        protected virtual void OnDestroy()
        {
            //Log.Sys($"{typeof(T).ToString()}: OnDestroy", caller: false);
        }

        /// <summary>
        /// Instance를 생성하기 위한 빈 메소드.
        /// </summary>
        public void EmptyMethod()
        {
        }
    }
}

#endif