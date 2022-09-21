using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Rano;

namespace Rano.Network
{
    public enum NetworkState
    {
        Unknown = -1,
        Disconnected = 0,
        Connected = 1
    }
    
    public sealed class NetworkManager : ManagerComponent
    {
        private NetworkState _state;

        [Header("Ping")]
        public string pingAddress = "8.8.8.8";
        public float pingWaitTime = 0.5f;
        public float pingNextTime = 3.0f;

        [Header("Event Callbacks")]
        public Action onConnected;
        public Action onDisconnected;

        public NetworkState State => _state;
        public bool IsConnected => _state == NetworkState.Connected;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(nameof(UpdateCoroutine));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopCoroutine(nameof(UpdateCoroutine));
        }
        
        private IEnumerator UpdateCoroutine()
        {
            Ping ping;
            _state = NetworkState.Unknown;
            
            while (true)
            {
                ping = new Ping(pingAddress);
                yield return CoroutineYieldCache.WaitForSeconds(pingWaitTime);

                if (ping.isDone && ping.time >= 0)
                {
                    // 핑을 받으면 연견된 것으로 간주한다.
                    if (_state == NetworkState.Unknown || _state == NetworkState.Disconnected)
                    {
                        Log.Sys("네트워크 연결됨");
                        _state = NetworkState.Connected;
                        onConnected?.Invoke();
                    }
                }
                else
                {
                    // 핑을 받지 못했거나 시간을 초과했다면 끊어진 것으로 간주한다.
                    if (_state == NetworkState.Unknown || _state == NetworkState.Connected)
                    {
                        Log.Sys("네트워크 끊어짐");
                        _state = NetworkState.Disconnected;
                        onDisconnected?.Invoke();
                    }
                }

                // 잠시 후에 재시도한다.
                yield return CoroutineYieldCache.WaitForSeconds(pingNextTime);
            }
        }

        /// <summary>
        /// 최초 네트워크 테스트가 완료될 때까지 대기한다.
        /// </summary>
        public async Task WaitForConnectionTestCompletedAsync()
        {
            Log.Info("네트워크 연결 테스트 중...");
            while (_state == NetworkState.Unknown)
            {
                await Task.Delay(25);
            }
            Log.Info("네트워크 연결 테스트가 완료되었습니다");
        }
    }
}