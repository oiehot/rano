using UnityEngine;
using System.Collections;

namespace Rano.Animation
{
    public abstract class SimpleAnimator : MonoBehaviour
    {
        private enum EState
        {
            Stopped,
            Playing
        }

        private EState _state = EState.Stopped;
        [SerializeField] private bool _playOnStart;

        protected virtual void Start()
        {
            if (_playOnStart)
            {
                Play();
            }
        }

        public void Play()
        {
            StartCoroutine(nameof(PlayCoroutine));
        }
        
        public void Stop()
        {
            _state = EState.Stopped;
        }

        private IEnumerator PlayCoroutine()
        {
            HandleStart();
            _state = EState.Playing;
            while (_state == EState.Playing)
            {
                HandleUpdate();
                yield return null;
            }
            HandleEnd();
        }

        protected virtual void HandleStart()
        {
        }

        protected virtual void HandleUpdate()
        {
        }

        protected virtual void HandleEnd()
        {
        }
    }
}