// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Rano;

namespace Rano.PlatformServices.Gaming
{
    public sealed class GameServiceManager : MonoSingleton<GameServiceManager>
    {
        public bool IsFeatureAvailable => GameServices.IsAvailable();
        private bool _lastResult = false;
        public bool IsAuthWorking { get; private set; }
        public bool IsAvailable
        {
            get
            {
                if (IsFeatureAvailable && GameServices.IsAuthenticated) return true;
                else return false;
            }
        }

        public ILocalPlayer LocalPlayer
        {
            get
            {
                return GameServices.LocalPlayer;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameServices.OnAuthStatusChange += OnAuthStatusChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameServices.OnAuthStatusChange -= OnAuthStatusChange;
        }

        private void OnAuthStatusChange(GameServicesAuthStatusChangeResult result, Error error)
        {
            if (error == null)
            {
                Log.Info($"게임서비스 인증상태 변경: ({result.AuthStatus})");

                if (result.AuthStatus == LocalPlayerAuthStatus.Authenticating)
                {
                    // Wait
                }
                else if (result.AuthStatus == LocalPlayerAuthStatus.Authenticated)
                {
                    Log.Info("게임서비스 로컬사용자: " + result.LocalPlayer);
                    _lastResult = true;
                    IsAuthWorking = false;
                }
                else if (result.AuthStatus == LocalPlayerAuthStatus.NotAvailable)
                {
                    Log.Warning("게임서비스 로컬사용자 사용불가능");
                    _lastResult = false;
                    IsAuthWorking = false;
                }
                else
                {
                    throw new Exception("게임서비스 인증상태에서 알수없는 LocalPlayerAuthStatus이 나옴.");
                    //_lastResult = false;
                    //IsAuthWorking = false;
                }
            }
            else
            {
                // TODO: => Log.Warning
                Log.Important($"게임서비스 로그인 실패 ({error})");
                _lastResult = false;
                IsAuthWorking = false;
            }
        }

        public void Authenticate(Action<bool> onResult=null)
        {
            StartCoroutine(AuthenticateCoroutine(onResult));
        }

        public IEnumerator AuthenticateCoroutine(Action<bool> onResult=null)
        {
            if (IsAuthWorking == true) yield break;
            IsAuthWorking = true;

            Log.Info("게임서비스 로그인 요청.");
            GameServices.Authenticate();
            Log.Info($"게임서비스 로그인 절차 종료A");
            while (IsAuthWorking == true)
            {
                yield return null;
            }
            Log.Info($"게임서비스 로그인 절차 종료B");
            onResult?.Invoke(_lastResult);
            Log.Info($"게임서비스 로그인 절차 종료C");
        }
    }
}