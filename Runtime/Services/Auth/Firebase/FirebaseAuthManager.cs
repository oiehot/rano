#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace Rano.Services.Auth
{   
    public sealed partial class FirebaseAuthManager : ManagerComponent
    {
        private FirebaseAuth? _auth;
        private FirebaseUser? _user;
        private FirebaseAnonymousAuth? _anonymousAuth;
        // private FirebaseEmailAuth? _emailAuth;
        // private FirebaseGpgsAuth? _gpgsAuth;
        // private FirebaseGameCenterAuth? _gameCenterAuth;
        public FirebaseAuth? Auth => _auth;
        public FirebaseAnonymousAuth? Anonymous => _anonymousAuth;
        // public FirebaseEmailAuth? Email => _emailAuth;
        // public FirebaseGpgsAuth Gpgs => _gpgsAuth;
        // public FirebaseGameCenterAuth GameCenter => _gameCenterAuth;
        public bool IsInitialized => _auth != null;
        public bool IsSignedIn
        {
            get
            {
                if (_auth != null && _auth.CurrentUser != null)
                {
                    return true;
                }
                else return false;
            }
        }
        // public IAuthUser? User
        // {
        //     get
        //     {
        //         if (_user != null) return new FirebaseAuthUser(_user);
        //         else return null;
        //     }
        // }
        public string? UserId
        {
            get
            {
                if (_auth != null && _auth.CurrentUser != null)
                {
                    return _auth.CurrentUser.UserId;
                }
                else return null;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _anonymousAuth = new FirebaseAnonymousAuth(this);
            // _emailAuth = new FirebaseEmailAuth(this);
            // _gpgsAuth = new FirebaseGpgsAuth(this);
            // _gameCenterAuth = new FirebaseGameCenterAuth(this);
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

        /// <summary>
        /// Firebase를 초기화한다.
        /// </summary>
        /// <remarks>
        /// 이전에 SignIn했었다면, 다시 SignIn하지 않더라도
        /// _auth.CurrentUser에 로그인된 사용자가 남아있다.
        /// </remarks>
        private void Initialize()
        {
            try
            {
                _auth = FirebaseAuth.DefaultInstance;
            }
            catch
            {
                Log.Warning(AuthMessages.AUTHMANAGER_INIT_FAILED);
                _auth = null;
                return;
            }
            _auth.StateChanged += HandleAuthStateChanged;
            HandleAuthStateChanged(this, null);
        }
        
        private void HandleAuthStateChanged(object sender, System.EventArgs? eventArgs)
        {
            if (_auth == null)
            {
                Log.Warning(AuthMessages.AUTHMANGER_IS_NOT_INITIALIZED);
                return;
            }
            
            if (_auth.CurrentUser != _user)
            {
                bool signedIn = (_user != _auth.CurrentUser) && (_auth.CurrentUser != null);
                if (!signedIn && _user != null)
                {
                    Log.Important(AuthMessages.SIGNED_OUT);
                }
                _user = _auth.CurrentUser;
                if (signedIn)
                {
                    Log.Important($"{AuthMessages.SIGNED_IN} (Id:{_user!.UserId})");
                }
            }
        }

        /// <summary>
        /// 인증 프로파이더로 부터 받은 자격증명을 통해 Sign In한다.
        /// </summary>
        /// <param name="credential">인증플랫폼으로 받은 자격증명</param>
        public async Task<bool> SignInWithCredentialAsync(Credential credential)
        {
            if (_auth == null)
            {
                Log.Warning(AuthMessages.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.SignInWithCredentialAsync(credential);
            }
            catch (Exception e)
            {
                Log.Warning(AuthMessages.SIGN_IN_WITH_CREDENTIAL_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Important(AuthMessages.SIGN_IN_WITH_CREDENTIAL_SUCCESS);
            return true;
        }

        /// <summary>
        /// 현재 SignIn된 Firebase계정에 플랫폼으로 받은 자격증명을 연결시킨다.
        /// </summary>
        /// <param name="credential">플랫폼으로 받은 자격증명</param>
        public async Task<bool> LinkWithCredentialAsync(Credential credential)
        {
            if (_auth == null)
            {
                Log.Warning(AuthMessages.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            if (_auth.CurrentUser == null)
            {
                Log.Warning(AuthMessages.CREDENTIAL_LINK_FAILED_NOT_SIGNED_IN);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.CurrentUser.LinkWithCredentialAsync(credential);
            }
            catch (Exception e)
            {
                Log.Warning(AuthMessages.CREDENTIAL_LINK_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Info(AuthMessages.CREDENTIAL_LINK_SUCCESS);
            return true;
        }

        public async Task<bool> UnlinkProviderAsync(string providerStr)
        {
            if (_auth == null)
            {
                Log.Warning(AuthMessages.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            if (_auth.CurrentUser == null)
            {
                Log.Warning(AuthMessages.CREDENTIAL_UNLINK_FAILED_NOT_SIGNED_IN);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.CurrentUser.UnlinkAsync(providerStr);
            }
            catch (Exception e)
            {
                Log.Warning(AuthMessages.CREDENTIAL_UNLINK_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Important(AuthMessages.CREDENTIAL_UNLINK_SUCCESS);
            return true;
        }
        
        /// <summary>
        /// SignOut한다.
        /// </summary>
        /// <remarks>익명인 경우 SignOut되면 UUID가 갱신되어 계정 정보가 분실된다.</remarks>
        public void SignOut()
        {
            if (_auth == null)
            {
                Log.Warning(AuthMessages.AUTHMANGER_IS_NOT_INITIALIZED);
                return;
            }
            
            if (_user != null)
            {
                try
                {
                    _auth.SignOut();
                }
                catch (Exception e)
                {
                    Log.Warning(AuthMessages.SIGN_OUT_ERROR);
                    Log.Exception(e);
                    return;
                }
            }
            else
            {
                Log.Info(AuthMessages.SIGNED_OUT_ALREADY);
            }
        }
        
        public void LogStatus()
        {
            Log.Info($"{nameof(FirebaseAuthManager)}:");
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