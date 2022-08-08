using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
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

        /// <summary>네트워크 연결상태를 리턴한다.</summary>
        /// <example>
        /// networkManager.Status == NetworkStatus.Connected;
        /// </example>
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
                    /* Ping recevied,  Set to Connected */
                    if (_state == NetworkState.Unknown || _state == NetworkState.Disconnected)
                    {
                        Log.Sys("Network Connected");
                        _state = NetworkState.Connected;
                        onConnected?.Invoke();
                    }
                }
                else
                {
                    /* Ping not received or timeout, Set to Disconnected */
                    if (_state == NetworkState.Unknown || _state == NetworkState.Connected)
                    {
                        Log.Sys("Network Disconnected");
                        _state = NetworkState.Disconnected;
                        onDisconnected?.Invoke();
                    }
                }

                // Retry after a while
                yield return CoroutineYieldCache.WaitForSeconds(pingNextTime);
            }
        }

        /// <summary>
        /// 최초 네트워크 테스트가 완료될 때까지 대기한다.
        /// </summary>
        public async Task WaitForConnectionTestCompletedAsync()
        {
            Log.Info("Wait for the network connection test to complete.", caller:false);
            while (_state == NetworkState.Unknown)
            {
                await Task.Delay(25);
            }
            Log.Info("Network connection test completed.",caller:false);
        }
    }
}