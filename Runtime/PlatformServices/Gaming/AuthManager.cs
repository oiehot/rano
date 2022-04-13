using System;
using System.Collections;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Rano;

namespace Rano.PlatformServices.Gaming
{
    public sealed class AuthManager : MonoSingleton<AuthManager>
    {
        private bool _lastResult = false;
        
        public bool IsFeatureAvailable => GameServices.IsAvailable();
        public bool IsAuthWorking { get; private set; }
        public bool IsAuthenticated => GameServices.IsAuthenticated;
        public ILocalPlayer LocalPlayer => GameServices.LocalPlayer;

        protected override void OnEnable()
        {
            base.OnEnable();
            GameServices.OnAuthStatusChange += HandleAuthStatusChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameServices.OnAuthStatusChange -= HandleAuthStatusChange;
        }

        private void HandleAuthStatusChange(GameServicesAuthStatusChangeResult result, Error error)
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
                Log.Warning($"게임서비스 로그인 실패 ({error})");
                _lastResult = false;
                IsAuthWorking = false;
            }
        }
        
        public IEnumerator AuthenticateCoroutine(Action<bool> onResult = null)
        {
            if (IsAuthWorking == true) yield break;
            IsAuthWorking = true;
            Log.Info("게임서비스 로그인 요청.");
            GameServices.Authenticate();
            yield return new WaitUntil(() => IsAuthWorking == true);

        onResult?.Invoke(_lastResult);
        }

        /// <summary>
        /// 게임서비스 로그인
        /// </summary>
        public void Authenticate(Action<bool> onResult=null)
        {
            StartCoroutine(AuthenticateCoroutine(onResult));
        }

        /// <summary>
        /// 게임서비스 로그아웃
        /// </summary>
        public void Signout()
        {
            Log.Info("게임서비스 로그아웃 요청.");
            GameServices.Signout();
        }
    }
}