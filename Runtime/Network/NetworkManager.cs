// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Rano;

namespace Rano
{
    public class NetworkManager : MonoSingleton<NetworkManager>
    {
        public bool isConnected { get; private set; }

        [Header("Ping")]
        public string pingAddress = "8.8.8.8";
        public float pingWaitTime = 0.5f;
        public float pingNextTime = 5.0f;

        [Header("Events")]
        public UnityEvent onConnected;
        public UnityEvent onDisconnected;

        void Awake()
        {
            isConnected = false;
        }

        void OnEnable()
        {
            Log.Info("NetworkManager Enabled");
            StartCoroutine(nameof(CoUpdate));
        }

        void OnDisable()
        {
            Log.Info("NetworkManager Disabled");
            StopCoroutine(nameof(CoUpdate));
        }

        IEnumerator CoUpdate()
        {
            Ping ping;

            while (true)
            {
                // 핑 발사
                ping = new Ping(pingAddress);

                // 핑 대기
                yield return new WaitForSeconds(pingWaitTime);

                if (ping.isDone)
                {
                    if (ping.time >=0)
                    {
                        // OFF > ON
                        if (isConnected == false)
                        {
                            onConnected.Invoke();
                            isConnected = true;
                        }
                    }
                    else
                    {
                        // Ping 실패 (-1 리턴)
                        // ON >> OFF
                        if (isConnected)
                        {
                            onDisconnected.Invoke();
                            isConnected = false;
                        }
                    }
                }
                else
                {
                    // Timeout 으로 인한 Ping 실패
                    if (isConnected)
                    {
                        onDisconnected.Invoke();
                        isConnected = false;
                    }
                }

                // 다음 시도 기다림
                yield return new WaitForSeconds(pingNextTime);
            }
        }
    }
}