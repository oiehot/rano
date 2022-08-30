#if UNITY_IOS

#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace Rano.Services.Auth
{
    public sealed class FirebaseGameCenterAuth : IAuthModule
    {
        private bool _authenticated;
        private FirebaseAuthManager _authManager;
        public bool IsAuthenticated => GameCenterAuthProvider.IsPlayerAuthenticated();

        public FirebaseGameCenterAuth(FirebaseAuthManager authManager)
        {
            _authManager = authManager;
        }
            
        public void Initialize()
        {
        }
        
        public void Authenticate()
        {
            Social.localUser.Authenticate(success =>
            {
                _authenticated = success;
                if (success)
                {
                    Log.Important(AuthMessages.SOCIAL_AUTH_SUCCESS);
                }
                else
                {
                    Log.Important(AuthMessages.SOCIAL_AUTH_FAILED);
                }
            });
        }
        
        public Task SignIn()
        {
            Task<Credential>? credentialTask = GameCenterAuthProvider.GetCredentialAsync();

            credentialTask.ContinueWith(task =>
            {
                // Credential Task Result:
                if (!task.IsCompleted)
                    return;
            
                if(task.Exception != null)
                    Log.Warning("GC Credential Task - Exception: " + task.Exception.Message);
            
                var credential = task.Result;
            });

            var continueTask = credentialTask.ContinueWithOnMainThread(task => {

            
                var loginTask = _authManager.Auth.SignInWithCredentialAsync(credential);
                return loginTask.ContinueWithOnMainThread(HandleSignInWithUser);
            });
            
            return continueTask;
        }
    }
}

#endif