#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace Rano.Auth.Firebase
{   
    public sealed class FirebaseAuthManager : ManagerComponent, IAuthManager
    {
        private FirebaseAuth? _auth;
        private FirebaseUser? _user;
        public FirebaseAuth? Auth => _auth;
        public bool IsInitialized => _auth != null;
        public bool IsAuthenticated
        {
            get
            {
                if (_auth != null && _auth.CurrentUser != null) return true;
                else return false;
            }
        }
        public string? UserId => IsAuthenticated ? _auth!.CurrentUser.UserId : null;

        /// <summary>
        /// 사용자의 표시 이름.
        /// </summary>
        /// <remarks>
        /// 이름을 지정하지 않았거나 익명이면 "" 값이 리턴된다.
        /// 인증이 안되어 있으면 "" 값이 리턴된다.
        /// </remarks>
        public string UserDisplayName => IsAuthenticated ? _auth!.CurrentUser.DisplayName : "";

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
        public bool Initialize()
        {
            try
            {
                _auth = FirebaseAuth.DefaultInstance;
            }
            catch
            {
                Log.Warning(Constants.AUTHMANAGER_INIT_FAILED);
                _auth = null;
                return false;
            }
            _auth.StateChanged += HandleAuthStateChanged;
            HandleAuthStateChanged(this, null);
            return true;
        }
        
        private void HandleAuthStateChanged(object sender, System.EventArgs? eventArgs)
        {
            if (_auth == null)
            {
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return;
            }
            
            if (_auth.CurrentUser != _user)
            {
                bool signedIn = (_user != _auth.CurrentUser) && (_auth.CurrentUser != null);
                if (!signedIn && _user != null)
                {
                    Log.Important(Constants.SIGNED_OUT);
                }
                _user = _auth.CurrentUser;
                if (signedIn)
                {
                    Log.Important($"{Constants.SIGNED_IN} (Id:{_user!.UserId})");
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
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.SignInWithCredentialAsync(credential);
            }
            catch (Exception e)
            {
                Log.Warning(Constants.SIGN_IN_WITH_CREDENTIAL_ERROR);
                Log.Exception(e);
                return false;
            }
            
            Log.Important(Constants.SIGN_IN_WITH_CREDENTIAL_SUCCESS);
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
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            if (_auth.CurrentUser == null)
            {
                Log.Warning(Constants.CREDENTIAL_LINK_FAILED_NOT_SIGNED_IN);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.CurrentUser.LinkWithCredentialAsync(credential);
            }
            catch (Exception e)
            {
                Log.Warning(Constants.CREDENTIAL_LINK_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Info(Constants.CREDENTIAL_LINK_SUCCESS);
            return true;
        }

        public async Task<bool> UnlinkProviderAsync(string providerStr)
        {
            if (_auth == null)
            {
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            if (_auth.CurrentUser == null)
            {
                Log.Warning(Constants.CREDENTIAL_UNLINK_FAILED_NOT_SIGNED_IN);
                return false;
            }
            try
            {
                FirebaseUser user = await _auth.CurrentUser.UnlinkAsync(providerStr);
            }
            catch (Exception e)
            {
                Log.Warning(Constants.CREDENTIAL_UNLINK_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Important(Constants.CREDENTIAL_UNLINK_SUCCESS);
            return true;
        }
        
        /// <summary>
        /// SignOut한다.
        /// </summary>
        /// <remarks>익명인 경우 SignOut되면 UUID가 갱신되어 계정 정보가 분실된다.</remarks>
        public bool SignOut()
        {
            if (_auth == null)
            {
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            
            if (_user != null)
            {
                try
                {
                    _auth.SignOut();
                }
                catch (Exception e)
                {
                    Log.Warning(Constants.SIGN_OUT_ERROR);
                    Log.Exception(e);
                    return false;
                }
            }
            else
            {
                Log.Info(Constants.SIGNED_OUT_ALREADY);
            }
            return true;
        }
        
        public void LogUser(FirebaseUser user)
        {
            Log.Info($"User:");
            Log.Info($"  UserId: {user.UserId}");
            Log.Info($"  DisplayName: {user.DisplayName}");
            Log.Info($"  Email: {user.Email}");
            Log.Info($"  PhoneNumber: {user.PhoneNumber}");            
            Log.Info($"  IsAnonymous: {user.IsAnonymous}");
            Log.Info($"  IsEmailVerified: {user.IsEmailVerified}");
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