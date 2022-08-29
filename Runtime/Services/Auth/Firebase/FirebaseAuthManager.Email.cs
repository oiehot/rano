#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Rano.Services.Auth
{
    public sealed partial class FirebaseAuthManager
    {
        public void CreateUserWithEmailAsync(string email, string password)
        {
            Debug.Assert(_auth != null);
            Debug.Assert(String.IsNullOrEmpty(email) == false);
            Debug.Assert(String.IsNullOrEmpty(password) == false);
            Log.Sys($"CreateUserWithEmailAsync Progress... (email:{email}, pw:{password})");
            //
            // Task<FirebaseUser> task = _auth!.CreateUserWithEmailAndPasswordAsync(email, password);
            // await task;
            //
            // if (task.IsCanceled) {
            //     Log.Warning("CreateUserWithEmailAsync was canceled.");
            //     return;
            // }
            //
            // if (task.IsFaulted)
            // {
            //     Log.Warning("CreateUserWithEmailAsync Exception occured");
            //     Log.Exception(task.Exception);
            //     return;
            // }
            //
            // FirebaseUser user = task.Result;
            // Log.Important($"CreateUserWithEmailAsync Success (DisplayName:{user.DisplayName}, Id:{user.UserId})");
            
            _auth!.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                // Firebase.Auth.FirebaseUser newUser = task.Result;
                _user = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                
                Log.Warning("Verification email sending...");
                _user.SendEmailVerificationAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled) {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted) {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                        return;
                    }
                    Log.Important("Verification Email Sent!");
                });
            });
        }

        private Credential GetEmailCredential(string email, string password)
        {
            Log.Info("Get Email Credential Progress...");
            Credential credential;
            credential = EmailAuthProvider.GetCredential(email, password);
            return credential;
        }

        private void LinkWithCredentialAsync(Credential credential)
        {
            Log.Info("Link CurrentAccount with GivenCredential Progress...");
            _auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("LinkWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }

        public void LinkCredentialWithEmail(string email, string password)
        {
            Credential credential = GetEmailCredential(email, password);
            LinkWithCredentialAsync(credential);
        }

        private void SignInWithCredentialAsync(Credential credential)
        {
            Log.Info("SignIn with Credential Progress...");
            _auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
        
        public void SignInWithEmailCredential(string email, string password)
        {
            Log.Info("SignIn with Email Credential Progress...");
            Credential credential = GetEmailCredential(email, password);
            SignInWithCredentialAsync(credential);
        }
        
        public void SignInWithEmailAsync(string email, string password)
        {
            Debug.Assert(_auth != null);
            Debug.Assert(String.IsNullOrEmpty(email) == false);
            Debug.Assert(String.IsNullOrEmpty(password) == false);
            // Log.Sys("SignInWithEmail Progress...");
            // EAuthState previousState = _state;
            // _state = EAuthState.SignInProgress;
            //
            // Task<FirebaseUser> task = _auth.SignInWithEmailAndPasswordAsync(email, password);
            // await task;
            //     
            // if (task.IsCanceled)
            // {
            //     Log.Warning($"SignInWithEmail Canceled");
            //     _state = EAuthState.SignedOut;
            //     return;
            // }
            //
            // if (task.IsFaulted)
            // {
            //     Log.Warning($"SignInWithEmail Failed ({task.Exception})");
            //     _state = EAuthState.SignedOut;
            //     return;
            // }
            //
            // _user = task.Result;
            // Log.Important($"SignInWithEmail Success (DisplayName:{_user.DisplayName}, Id:{_user.UserId})");
            _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                _user = task.Result;
                if (_user.IsEmailVerified)
                {
                    Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                }
                else
                {
                    Log.Warning("Email verification required!");
                }
            });
        }
    }
}