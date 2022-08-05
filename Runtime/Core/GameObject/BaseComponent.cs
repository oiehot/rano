using UnityEngine;

namespace Rano
{
    public class BaseComponent : MonoBehaviour
    {
        private string ComponentName => GetType().Name;
        
        protected virtual void Awake()
        {
            Log.Sys($"{ComponentName}: Awake", caller: false);
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