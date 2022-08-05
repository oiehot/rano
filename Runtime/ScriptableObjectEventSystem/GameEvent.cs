using System.Collections.Generic;
using UnityEngine;

namespace Rano.ScriptableObjectEventSystem
{
    /// <summary>
    /// 이벤트가 발생했을 때 호출 할 GameEventListener 들을 담아두고
    /// 이벤트를 발생시키면 일괄적으로 호출한다.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Rano/ScriptableObjectEventSystem/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> _listeners = new List<GameEventListener>();
        public List<GameEventListener> Listeners => _listeners;

        public void Register(GameEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void Unregister(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }

        public void Raise()
        {
            for (int i=_listeners.Count-1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }
    }
}