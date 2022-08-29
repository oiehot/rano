#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace Rano.Services.Auth
{   
    public sealed partial class FirebaseAuthManager : ManagerComponent
    {
        private EAuthState _state = EAuthState.Initializing;
        private FirebaseAuth? _auth;
        private FirebaseUser? _user;
        public bool IsInitializing => _state == EAuthState.Initializing;

        public IAuthUser? User
        {
            get
            {
                if (_user != null) return new FirebaseAuthUser(_user);
                else return null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_auth != null)
            {
                _auth.StateChanged -= HandleAuthStateChanged;
            }
            _user = null;
            _auth = null;
        }

        private void Initialize()
        {
            _state = EAuthState.Initializing;
            try
            {
                _auth = FirebaseAuth.DefaultInstance;
            }
            catch
            {
                Log.Warning("FirebaseAuth 초기화 실패");
                _auth = null;
                _state = EAuthState.SignedOut;
                return;
            }
            _auth.StateChanged += HandleAuthStateChanged;
            HandleAuthStateChanged(this, null);
        }
        
        private void HandleAuthStateChanged(object sender, System.EventArgs? eventArgs)
        {
            if (_auth == null)
            {
                Log.Warning("A state change event occurred even though it is not in the initialized state. It cannot be processed.");
                return;
            }
            
            if (_auth.CurrentUser != _user)
            {
                bool signedIn = (_user != _auth.CurrentUser) && (_auth.CurrentUser != null);
                if (!signedIn && _user != null)
                {
                    Log.Important($"Signed out");
                    _state = EAuthState.SignedOut;
                }
                _user = _auth.CurrentUser;
                if (signedIn)
                {
                    Log.Important($"Signed in (Id:{_user!.UserId})");
                    _state = EAuthState.SignedIn;
                }
            }
        }

        /// <summary>
        /// 익명으로 로그인한다.
        /// </summary>
        public async Task SignInAnonymousAync()
        {
            if (_auth == null)
            {
                Log.Warning("Authentication is not initialized. Can't Sign In");
                return;
            }
            
            if (_auth.CurrentUser != null)
            {
                Log.Info("Already signed in. Skip the SignIn");
                return;
            }
                
            Log.Sys("SignIn by Anonymous Progress...");
            EAuthState previousState = _state;
            _state = EAuthState.SignInProgress;

            Task<FirebaseUser> task;
            try
            {
                task = _auth.SignInAnonymouslyAsync();
                await task;
            }
            catch (Exception e)
            {
                Log.Warning("SignIn by Anonymous Exception occured");
                Log.Exception(e);
                _state = previousState;
                return;
            }

            if (task.IsCanceled)
            {
                Log.Warning($"SignIn by Anonymous Canceled");
                _state = EAuthState.SignedOut;
                return;
            }

            if (task.IsFaulted)
            {
                Log.Warning($"SignIn by Anonymous Failed ({task.Exception})");
                _state = EAuthState.SignedOut;
                return;
            }

            _user = task.Result;
            Log.Important($"SignIn by Anonymous Success (DisplayName:{_user.DisplayName}, Id:{_user.UserId})");
        }

        /// <summary>
        /// 로그 아웃한다.
        /// </summary>
        /// <remarks>익명인 경우 SignOut되면 UUID가 갱신되어 계정 정보가 분실된다.</remarks>
        public void SignOut()
        {
            if (_auth == null)
            {
                Log.Warning("Authentication is not initialized. Can't Sign Out");
                return;
            }
            
            if (_user != null)
            {
                EAuthState previousState = _state;
                _state = EAuthState.SignOutProgress;
                try
                {
                    _auth.SignOut();
                }
                catch (Exception e)
                {
                    Log.Warning("An exception occurred during SignOut");
                    Log.Exception(e);
                    _state = previousState;
                    return;
                }
                Log.Sys($"Sign out (Id:{_user.UserId})");
            }
            else
            {
                Log.Info("Already signed out. Skip the SignOut");
            }
        }
        
        public void LogStatus()
        {
            Log.Info("FirebaseAuthManager:");
            Log.Info($"  State: {_state}");
            Log.Info($"  IsInitializing: {IsInitializing}");
            Log.Info($"  FirebaseAuth: {_auth}");
            Log.Info($"  FirebaseUser: {_user}");
            if (_user != null)
            {
                Log.Info($"    Id: {_user.UserId}");
                Log.Info($"    DisplayName: {_user.DisplayName}");
                Log.Info($"    Email: {_user.Email}");
                Log.Info($"    PhotoUrl: {_user.PhotoUrl}");
                Log.Info($"    PhoneNumber: {_user.PhoneNumber}");
                Log.Info($"    ProviderId: {_user.ProviderId}");
                Log.Info($"    ProviderData:");
                foreach (var provider in _user.ProviderData)
                {
                    Log.Info($"      - {provider.ProviderId}");
                    Log.Info($"          Email: {provider.Email}");
                    Log.Info($"          DisplayName: {provider.DisplayName}");
                    Log.Info($"          PhotoUrl: {provider.PhotoUrl}");
                    Log.Info($"          UserId: {provider.UserId}");
                }
                Log.Info($"    IsAnonymous: {_user.IsAnonymous}");
                Log.Info($"    IsEmailVerified: {_user.IsEmailVerified}");
                Log.Info($"    MetaData: {_user.Metadata}");
                Log.Info($"      CreationTimestamp: {_user.Metadata.CreationTimestamp}");
                Log.Info($"      LastSignInTimestamp: {_user.Metadata.LastSignInTimestamp}");
            }
        }
    }
}