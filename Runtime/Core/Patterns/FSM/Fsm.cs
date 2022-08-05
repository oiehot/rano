using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.Pattern
{
    public class Fsm<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        private readonly MonoBehaviour _mono;
        private readonly Dictionary<T, Func<IEnumerator>> _states =
            new Dictionary<T, Func<IEnumerator>>();
        
        private Coroutine _runningCoroutineHandle;
        private Coroutine _stateCoroutineHandle;

        public bool isRunning {get; private set;}
        public T current {get; private set;}
        public T next {get; set;} // 이 값을 설정하면 곧 상태가 바뀐다.

        public bool shouldChange => !EqualityComparer<T>.Default.Equals(current, next);

        public Fsm(MonoBehaviour mono)
        {
            if (ReferenceEquals(mono, null))
            {
                throw new ArgumentNullException();
            }
            _mono = mono;
            isRunning = false;

            // T의 모든 요소들을 키로 하여 null 값으로 _states 초기화.
            // 추후 _states 에 키로 등록되어있는 것만 RegisterStateCoroutine 가능하다.
            var t = typeof(T);
            var names = Enum.GetNames(t);
            foreach (var name in names)
            {
                InitState((T)Enum.Parse(t, name), null);
            }
        }

        private void InitState(T state, Func<IEnumerator> func)
        {
            if (_states.ContainsKey(state))
            {
                throw new StateAlreadyContainedException();
            }
            _states.Add(state, func);
        }

        public void RegisterStateCoroutine(T state, Func<IEnumerator> func)
        {
            // InitState() 를 통해 등재되지 않은 키는 코루틴 함수를 추가할 수 없다.
            if (!_states.ContainsKey(state))
            {
                throw new InvalidStateException();
            }
            _states[state] = func;
        }

        public void Run(T initialState)
        {
            if (isRunning)
            {
                Log.Warning("Already started.");
                return;
            }

            if (!_states.ContainsKey(initialState))
            {
                throw new InvalidStateException();
            }

            isRunning = true;
            current = initialState;
            next = current;

            StopCoroutine(ref _runningCoroutineHandle);
            _runningCoroutineHandle = _mono.StartCoroutine(RunningCoroutine());
        }

        public void Kill()
        {
            StopCoroutine(ref _stateCoroutineHandle);
            StopCoroutine(ref _runningCoroutineHandle);
            isRunning = false;
        }

        private IEnumerator RunningCoroutine()
        {
            while (isRunning)
            {
                var func = _states[current];

                if (!ReferenceEquals(func, null))
                {
                    // 상태 코루틴 실행
                    _stateCoroutineHandle = _mono.StartCoroutine(func());
                    yield return _stateCoroutineHandle; // 코루틴 종료될 때 까지
                }

                // 다음 상태가 설정될 때까지 대기
                while (isRunning && current.Equals(next))
                {
                    yield return null;
                }

                // 상태 코루틴 중지
                StopCoroutine(ref _stateCoroutineHandle);
                
                current = next;
                next = current;
            }
        }

        private void StopCoroutine(ref Coroutine coroutine)
        {
            if (ReferenceEquals(coroutine, null)) return;
            _mono.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public class FsmException : Exception
    {
        public FsmException()
        {
        }

        public FsmException(string message) : base(message)
        {
        }

        public FsmException(string message, Exception inner) : base(message,inner)
        {
        }
    }

    public class InvalidStateException : Exception
    {
        public InvalidStateException()
        {
        }

        public InvalidStateException(string message) : base(message)
        {
        }

        public InvalidStateException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class StateAlreadyContainedException : Exception
    {
        public StateAlreadyContainedException()
        {
        }

        public StateAlreadyContainedException(string message) : base(message)
        {
        }

        public StateAlreadyContainedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}