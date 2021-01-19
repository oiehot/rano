namespace Rano.Core
{
    using UnityEngine;

    // public class SingletonA<T> : MonoBehaviour where T : MonoBehaviour
    // {
    //     private static bool m_Closed = false;
    //     private static T m_Instance;
    //     public static T Instance
    //     {
    //         get
    //         {
    //             return m_Instance;
    //         }
    //     }

    //     private void Awake()
    //     {
    //         // var gameObject = new GameObject();
    //         // m_Instance = gameObject.AddComponent<T>();
    //         // gameObject.name = typeof(T).ToString() + " (Singleton)";
    //         // DontDestroyOnLoad(gameObject);
    //     }

    //     private void OnApplicationQuit()
    //     {
    //         m_Closed = true;
    //     }

    //     private void OnDestroy()
    //     {
    //         m_Closed = true;
    //     }
    // }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = (T)FindObjectOfType(typeof(T));
                        if (m_Instance == null)
                        {
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }

        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}