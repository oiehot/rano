#if ENABLE_GPGS

#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEditor;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Rano.Services.Auth
{
    public sealed class FirebaseGpgsAuth : IAuthModule
    {
        private readonly FirebaseAuthManager _authManager;
        private string? _authCode = null;
        private bool _authenticated;
        public bool IsAuthenticated => _authenticated;

        public FirebaseGpgsAuth(FirebaseAuthManager authManager)
        {
            _authManager = authManager;
        }
        
        public void Initialize()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .RequestServerAuthCode(false) /* Don't force refresh */
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
        }
        
        public void Authenticate()
        {
            Social.localUser.Authenticate(success =>
            {
                _authenticated = success;
                if (success)
                {
                    Log.Important(AuthMessages.SOCIAL_AUTH_SUCCESS);
                    _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                }
                else
                {
                    Log.Important(AuthMessages.SOCIAL_AUTH_FAILED);
                }
            });
        }

        private Credential? GetCredential()
        {
            if (String.IsNullOrEmpty(_authCode))
            {
                Log.Warning(AuthMessages.SOCIAL_IS_NOT_AUTHENTICATED);
                return null;
            }
            Credential? credential;
            try
            {
                credential = PlayGamesAuthProvider.GetCredential(_authCode);
            }
            catch (Exception e)
            {
                Log.Warning(AuthMessages.CANT_GET_CREDENTIAL);
                Log.Exception(e);
                return null;
            }
            return credential;
        }

        public async void SignIn()
        {
            Log.Todo("다음 케이스 테스트가 필요: GPGS Credential 로그인 실패 => 익명 로그인 => GPGS Credential 로그인 => 익명계정과 자동으로 Link 되었는가?");
            if (_authManager.Auth == null)
            {
                Log.Warning(AuthMessages.AUTH_IS_NOT_INITIALIZED_CANT_SIGN_IN);
                return;
            }
            if (_authenticated == false)
            {
                Log.Warning(AuthMessages.SOCIAL_IS_NOT_AUTHENTICATED);
                return;
            }
            Credential? credential = GetCredential();
            if (credential == null) return;
            _authManager.SignInWithCredentialAsync(credential);
        }
    }
}
#endif