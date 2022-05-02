using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Rano;

namespace Rano.PlatformServices.Gaming
{
    public sealed class AuthManager : MonoSingleton<AuthManager>
    {
        public Action OnAuthCompleted { get; set; }
        public Action OnAuthNotAvailable { get; set; }
        public Action OnAuthFailed { get; set; }
        
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
                    // Pass
                }
                else if (result.AuthStatus == LocalPlayerAuthStatus.Authenticated)
                {
                    Log.Info("게임서비스 로컬사용자: " + result.LocalPlayer);
                    IsAuthWorking = false;
                    OnAuthCompleted?.Invoke();
                }
                else if (result.AuthStatus == LocalPlayerAuthStatus.NotAvailable)
                {
                    Log.Warning("게임서비스 로컬사용자 사용불가능");
                    IsAuthWorking = false;
                    OnAuthNotAvailable?.Invoke();
                }
                else
                {
                    Log.Warning("게임서비스 인증상태에서 알수없는 LocalPlayerAuthStatus이 나옴.");
                    IsAuthWorking = false;
                }
            }
            else
            {
                Log.Warning($"게임서비스 로그인 실패 ({error})");
                IsAuthWorking = false;
                OnAuthFailed?.Invoke();
            }
        }
        
        public async Task AuthenticateAsync()
        {
            if (IsFeatureAvailable == false)
            {
                Log.Warning("게임서비스를 사용할 수 없어 로그인할 수 없습니다.");
                return;
            }
            if (IsAuthenticated)
            {
                Log.Warning("게임서비스에 이미 로그인 되어있습니다.");
                return;
            }
            
            if (IsAuthWorking)
            {
                Log.Warning("게임서비스가 이미 로그인중입니다.");
                return;
            }
            
            Log.Info("게임서비스 로그인 요청.");
            IsAuthWorking = true;
            GameServices.Authenticate();
            while (IsAuthWorking) await Task.Yield();
        }

        public void Signout()
        {
            Log.Info("게임서비스 로그아웃 요청.");
            GameServices.Signout();
        }
        
        public void LogStatus()
        {
            Log.Info("GamingServiceStatus:");
            Log.Info($"  FeatureAvailable: {IsFeatureAvailable}");
            Log.Info($"  AuthWorking: {IsAuthWorking}");
            Log.Info($"  Authenticated: {IsAuthenticated}");
        }
    }
}