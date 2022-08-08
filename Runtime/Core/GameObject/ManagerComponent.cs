using UnityEngine;

namespace Rano
{
    public class ManagerComponent : MonoBehaviour
    {
        private string ComponentName => GetType().Name;
        
        protected virtual void Awake()
        {
            Log.Sys($"{ComponentName}: Awake", caller: false);
            
            // 매니져 컴포넌트를 부착한 게임오브젝트는 씬 전환시 삭제되지 않아야 한다.
            GameObject.DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnEnable()
        {
            Log.Sys($"{ComponentName}: OnEnable", caller: false);
        }
        
        protected virtual void OnApplicationQuit()
        {
            Log.Sys($"{ComponentName}: OnApplicationQuit", caller: false);
        }

        protected virtual void OnDisable()
        {
            Log.Sys($"{ComponentName}: OnDisable", caller: false);
        }
        
        protected virtual void OnDestroy()
        {
            Log.Sys($"{ComponentName}: OnDestroy", caller: false);
        }
    }
}