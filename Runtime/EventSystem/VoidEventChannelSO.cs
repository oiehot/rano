#nullable enable

using System;
using UnityEngine;

namespace Rano.EventSystem
{
    [CreateAssetMenu(fileName="VoidEventChannel", menuName = "Rano/Events/Void Event Channel")]
    public sealed class VoidEventChannelSO : DescriptionBaseSO 
    {
        public Action? OnRaiseEvent { get; set; }
        
        public void RaiseEvent()
        {
            OnRaiseEvent?.Invoke();
        }
    }
}