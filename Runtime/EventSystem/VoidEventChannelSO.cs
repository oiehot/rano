#nullable enable

using System;
using UnityEngine;

namespace Rano.EventSystem
{
    [CreateAssetMenu(fileName="VoidEventChannel", menuName = "Rano/Events/Void Event Channel")]
    public sealed class VoidEventChannelSO : DescriptionBaseSO
    {
        public event Action? OnRaiseEvent;
        
        public void RaiseEvent()
        {
            OnRaiseEvent?.Invoke();
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 스크립터블 오브젝트의 상태를 출력한다.
        /// ScriptableObject 에셋에서 우클릭하면 사용할 수 있다.
        /// </summary>
        [ContextMenu("LogStatus")]
        private void LogStatus()
        {
            Log.Info($"{this}:");

            if (OnRaiseEvent == null) return;
            
            var subscribers = OnRaiseEvent.GetInvocationList();
            int subscriberCount = subscribers.Length;
            
            Log.Info($"  Subscribers:");
            
            if (subscriberCount > 0)
            {
                foreach (var invocation in subscribers)
                {
                    UnityEngine.Object? obj = invocation.Target as UnityEngine.Object;
                    if (obj == null) continue;
                    string objectName = obj.name;
                    string methodName = invocation.Method.Name;
                    string typeName = invocation.Target.GetType().Name;
                    Log.Info($"    - {objectName}.{methodName} ({typeName})");
                }
            }
            else
            {
                Log.Info("    (none)");
            }
        }
#endif        
    }
}