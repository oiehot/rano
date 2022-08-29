#if false

#nullable enable

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;

namespace Rano.Services.Auth
{
    public sealed class FirebaseGameCenterAuth
    {
        public void Authenticate()
        {
#if UNITY_IOS
        Social.localUser.Authenticate(success => {
          Log.Important($"Game Center Initialization Complete ({success})");
        });
#else
            Log.Warning("Game Center is not supported on this platform.");
#endif
        }

        private bool IsGameCenterAuthEnabled()
        {
#if UNITY_IOS
            return Firebase.Auth.GameCenterAuthProvider.IsPlayerAuthenticated()
#else
            return false;
#endif
        }

        public Task SignIn()
        {
            var credentialTask = Firebase.Auth.GameCenterAuthProvider.GetCredentialAsync();
            var continueTask = credentialTask.ContinueWithOnMainThread(task => {
                if(!task.IsCompleted)
                    return null;

                if(task.Exception != null)
                    Log.Warning("GC Credential Task - Exception: " + task.Exception.Message);

                var credential = task.Result;

                var loginTask = auth.SignInWithCredentialAsync(credential);
                return loginTask.ContinueWithOnMainThread(HandleSignInWithUser);
            });

            return continueTask;
        }
    }
}

#endif