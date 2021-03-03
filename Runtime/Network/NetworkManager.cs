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
    public class NetworkManager : Singleton<NetworkManager>, IService
    {
        public EServiceState state { get; private set; }
        public bool connected { get; private set; }

        public UnityAction OnNetworkConnected;
        public UnityAction OnNetworkDisconnected;
        private Coroutine checkNetworkCoroutine;

        void Awake()
        {
            this.state = EServiceState.None;
            this.connected = false;
        }

        void OnEnable()
        {
            this.Resume();
        }

        void OnDisable()
        {
            this.Pause();
        }

        public void Init()
        {
            Log.Info("NetworkManager Init");
            this.state = EServiceState.Initalized;
        }

        public void Run()
        {
            if (this.state == EServiceState.Initalized ||
                this.state == EServiceState.Stopped)
            {
                Log.Info("NetworkManager Run");
                this.state = EServiceState.Running;
                this.checkNetworkCoroutine = StartCoroutine(this.CheckNetworkCoroutine());
            }
        }

        public void Pause()
        {
            if (this.state == EServiceState.Running)
            {
                Log.Info("NetworkManager Pause");
                StopCoroutine(this.checkNetworkCoroutine);
                this.state = EServiceState.Paused;
            }
        }

        public void Resume()
        {
            if (this.state == EServiceState.Paused)
            {
                Log.Info("NetworkManager Resume");
                this.checkNetworkCoroutine = StartCoroutine(this.CheckNetworkCoroutine());
                this.state = EServiceState.Running;
            }
        }

        public void Stop()
        {
            if (this.state == EServiceState.Running)
            {
                Log.Info("NetworkManager Running > Stop");
                StopCoroutine(this.checkNetworkCoroutine);
                this.state = EServiceState.Stopped;
            }
            else if(this.state == EServiceState.Paused)
            {
                Log.Info("NetworkManager Pause > Stop");
                this.state = EServiceState.Stopped;
            }
        }

        IEnumerator CheckNetworkCoroutine()
        {
            const string pingAddress = "8.8.8.8";
            const float pingWaitTime = 0.5f;
            const float nextDelayTime = 5.0f;
            Ping ping;
            // float pingStartTime;

            while (this.state == EServiceState.Running)
            {
                ping = new Ping(pingAddress);
                // pingStartTime = Time.time;

                yield return new WaitForSeconds(pingWaitTime);

                if (ping.isDone)
                {
                    // Ping 성공
                    if (ping.time >=0)
                    {
                        // OFF > ON 인 경우
                        if (!this.connected)
                        {
                            this.OnNetworkConnected();
                            this.connected = true;
                        }
                    }
                    else
                    {
                        // Ping 실패 (-1 리턴)
                        // ON >> OFF 인 경우
                        if (this.connected)
                        {
                            this.OnNetworkDisconnected();
                            this.connected = false;
                        }
                    }
                }
                else
                {
                    // Ping 실패 (Timeout)
                    if (this.connected)
                    {
                        this.OnNetworkDisconnected();
                        this.connected = false;
                    }
                }

                // 첫번째 Ping인 경우 업데이트 시간 수정.
                yield return new WaitForSeconds(nextDelayTime);
            }
        }
    }
}