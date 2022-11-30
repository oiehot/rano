#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Rano.Auth.Firebase.Gpgs
{
    public sealed class FirebaseGpgsAuth : IFirebaseAuthModule
    {
        public enum EState
        {
            None = 0,
            Initializing,
            Initialized,
            Authenticating,
            Authenticated,
        }
        
        private EState _state;
        private FirebaseAuthManager? _authManager;
        private string? _authCode;
        
        public bool IsInitialized => _state >= EState.Initialized;
        public bool IsAuthenticated => _state >= EState.Authenticated;
        
        /// <summary>
        /// 초기화 한다.
        /// </summary>
        public void Initialize(FirebaseAuthManager authManager)
        {
            _state = EState.Initializing;
            Log.Info("GPGS인증 초기화 중...");
            
            // AuthManager 초기화가 되어 있어야만 한다.
            _authManager = authManager;
            
            if (_authManager.IsInitialized == false)
            {
                Log.Warning("GPGS 인증 초기화 실패 (AuthManager가 초기화 되어있지 않음");
                return;
            }
            
            // PlayGamesPlatform을 초기화 한다.
            try
            {
                PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                    .RequestServerAuthCode(false) /* Don't force refresh */
                    .Build();
                PlayGamesPlatform.InitializeInstance(config);
                PlayGamesPlatform.Activate();
            }
            catch (Exception e)
            {
                Log.Warning("GPGS 인증 초기화 실패 (PlayGamesPlatform 초기화 실패)");
                Log.Exception(e);
                _state = EState.None;
            }
            
            Log.Info("GPGS 인증 초기화 성공");
            _state = EState.Initialized;
        }
        
        /// <summary>
        /// GPGS 소셜 인증을 시도한다.
        /// </summary>
        private async Task<bool> AuthenticateAsync()
        {
            _state = EState.Authenticating;
            Log.Info("GPGS 인증 요청");
            Social.localUser.Authenticate(HandleSocialAuthenticate);
            
            Log.Info("GPGS 인증 중...");
            
            // 인증결과가 나올 때 까지 대기
            while (_state == EState.Authenticating) await Task.Yield();
            
            // 인증 결과가 나옴.
            if (_state == EState.Authenticated) return true;
            else return false;
        }

        /// <summary>
        /// GPGS 소셜 인증 결과 이벤트를 처리한다.
        /// </summary>
        private void HandleSocialAuthenticate(bool success)
        {
            _authCode = null;
            
            if (success)
            {
                Log.Info("GPGS 내부 인증 성공");
                Log.Info("GPGS 서버 인증 코드 얻는 중...");
                try
                {
                    _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                }
                catch (Exception e)
                {
                    Log.Warning("GPGS 서버 인증 코드를 얻는데 실패 (예외 발생)");
                    Log.Exception(e);
                    _state = EState.Initialized;
                    return;
                }
            }
            else
            {
                Log.Warning("GPGS 내부 인증 실패");
                _state = EState.Initialized;
                return;
            }

            if (String.IsNullOrEmpty(_authCode))
            {
                Log.Warning("GPGS 서버 인증 코드를 얻는데 실패 (빈 코드)");
                _state = EState.Initialized;
                return;
            }
            
            Log.Info("GPGS 서버 인증 코드 얻기 성공");
            _state = EState.Authenticated;
        }

        /// <summary>
        /// GPGS를 통해 SignIn한다.
        /// </summary>
        public async Task<bool> SignInAsync()
        {
            // 초기화 체크
            if (IsInitialized == false)
            {
                Log.Warning("로그인 실패 (초기화 안됨)");
                return false;
            }
            
            // Social 인증
            if (await AuthenticateAsync())
            {
                Log.Info("로그인 성공 (GPGS)");
            }
            else
            {
                Log.Warning("로그인 실패 (GPGS 인증 실패)");
                return false;
            }
            
            // GPGS 인증서 얻기
            Credential? credential;
            try
            {
                credential = PlayGamesAuthProvider.GetCredential(_authCode);
            }
            catch (Exception e)
            {
                Log.Warning("로그인 실패 (GPGS 인증서를 얻는 중 예외가 발생)");
                Log.Exception(e);
                return false;
            }
            if (credential == null)
            {
                Log.Warning("로그인 실패 (GPGS 인증서가 없음)");
                return false;
            }
            
            if (_authManager == null)
            {
                Log.Warning("로그인 실패 (AuthManager가 연결되어 있지 않음)");
                return false;
            }
            
            // GPGS 인증서를 통해 Firebase 로그인
            bool result = await _authManager.SignInWithCredentialAsync(credential);
            if (result) Log.Info("로그인 성공 (GPGS)");
            else Log.Warning("로그인 실패 (GPGS)");
            
            return result;
        }
    }
}