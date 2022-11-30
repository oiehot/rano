#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace Rano.Auth.Firebase
{
    public sealed class FirebaseAnonymousAuth : IFirebaseAuthModule
    {
        private enum EState
        {
            None = 0,
            Initializing,
            Initialized,
        }

        private EState _state = EState.None;
        private FirebaseAuthManager? _authManager;
        public bool IsInitialized => _state >= EState.Initialized;
        
        /// <summary>
        /// 초기화 한다.
        /// </summary>
        public void Initialize(FirebaseAuthManager authManager)
        {
            _state = EState.Initializing;
            
            Log.Info("Anonymous인증 초기화 중...");
            
            // AuthManager 초기화가 되어 있어야만 한다.
            _authManager = authManager;
            if (_authManager.IsInitialized == false)
            {
                Log.Warning("Anonymous인증 초기화 실패 (AuthManager가 초기화 되어있지 않음");
                return;
            }
            Log.Info("Anonymous인증 초기화 성공");
            _state = EState.Initialized;
        }

        /// <summary>
        /// Anonymous 방법으로 SignIn한다.
        /// </summary>
        public async Task<bool> SignInAsync()
        {
            FirebaseUser user;
            
            Log.Warning("로그인 중... (Anonymous)");
            
            if (IsInitialized == false)
            {
                Log.Warning("로그인 실패 (Anonymous 인증이 초기화 되어있지 않음)");
                return false;
            }

            if (_authManager == null)
            {
                Log.Warning("로그인 실패 (AuthManager가 연결되어 있지 않음)");
                return false;
            }

            if (_authManager.IsInitialized == false)
            {
                Log.Warning("로그인 실패 (AuthManager가 초기화되어 있지 않음)");
                return false;
            }
            
            try
            {
                user = await _authManager.Auth!.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Log.Warning("로그인 실패 (Anonymous 인증 실패)");
                Log.Exception(e);
                return false;
            }
            _authManager.LogUser(user);
            
            Log.Info("로그인 성공 (Anonymous)");
            return true;
        }
    }
}