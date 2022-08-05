using UnityEngine;
using UnityEngine.Events;

namespace Rano.ScriptableObjectEventSystem
{
    /// <summary>
    /// 특정 이벤트에 등록하고
    /// 이벤트가 발생하면 등록했던 메서드들을 실행한다.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent;
        [SerializeField] private UnityEvent _response;

        public GameEvent GameEvent => _gameEvent;
        public UnityEvent Response => _response;

        private void OnEnable()
        {
            _gameEvent.Register(this);
        }

        private void OnDisable()
        {
            _gameEvent.Unregister(this);
        }

        public void OnEventRaised()
        {
            _response.Invoke();
        }
    }
}