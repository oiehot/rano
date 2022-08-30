#if false

#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Rano.Services.Auth
{
    public sealed class FirebaseEmailAuth
    {
        private readonly FirebaseAuthManager _authManager;
        
        public FirebaseEmailAuth(FirebaseAuthManager authManager)
        {
            _authManager = authManager;
        }
        
        // public void CreateUserWithEmailAsync(string email, string password)
        // {
        //     Debug.Assert(_auth != null);
        //     Debug.Assert(String.IsNullOrEmpty(email) == false);
        //     Debug.Assert(String.IsNullOrEmpty(password) == false);
        //     Log.Sys($"CreateUserWithEmailAsync Progress... (email:{email}, pw:{password})");
        //
        //     _auth!.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
        //         if (task.IsCanceled) {
        //             Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        //             return;
        //         }
        //         if (task.IsFaulted) {
        //             Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        //             return;
        //         }
        //         
        //         // Firebase.Auth.FirebaseUser newUser = task.Result;
        //         _user = task.Result;
        //         Debug.LogFormat("Firebase user created successfully: {0} ({1})", _user.DisplayName, _user.UserId);
        //         
        //         Log.Warning("Verification email sending...");
        //         _user.SendEmailVerificationAsync().ContinueWith(task =>
        //         {
        //             if (task.IsCanceled) {
        //                 Debug.LogError("SendEmailVerificationAsync was canceled.");
        //                 return;
        //             }
        //             if (task.IsFaulted) {
        //                 Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
        //                 return;
        //             }
        //             Log.Important("Verification Email Sent!");
        //         });
        //     });
        // }

        private Credential? GetCredential(string email, string password)
        {
            if (String.IsNullOrEmpty(email)) return null;
            if (String.IsNullOrEmpty(password)) return null;
            Credential? credential;
            try
            {
                credential = EmailAuthProvider.GetCredential(email, password);
            }
            catch (Exception e)
            {
                Log.Warning(AuthMessages.CREDENTIAL_CANT_GET);
                Log.Exception(e);
                return null;
            }
            return credential;
        }
        
        public async void SignInAsync(string email, string password)
        {
            Credential? credential = GetCredential(email, password);
            if (credential == null) return;
            bool linkResult = await _authManager.LinkWithCredentialAsync(credential);
            if (linkResult == true)
            {
                await _authManager.SignInWithCredentialAsync(credential);
            }
            else
            {
                Log.Warning("증명연결에 실패하여 로그인하지 않겠습니다.");
            }
        }

        public async Task<bool> UnlinkAsync()
        {
            bool result = await _authManager.UnlinkProviderAsync(EmailAuthProvider.ProviderId);
            return result;
        }
    }
}

#endif