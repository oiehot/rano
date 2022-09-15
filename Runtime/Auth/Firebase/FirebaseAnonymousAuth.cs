#nullable enable

using System;
using System.Threading.Tasks;
using Firebase.Auth;

namespace Rano.Auth.Firebase
{
    public sealed class FirebaseAnonymousAuth : IAuthModule
    {
        private readonly FirebaseAuthManager _authManager;
        
        public FirebaseAnonymousAuth(FirebaseAuthManager authManager)
        {
            _authManager = authManager;
        }

        public async Task<bool> SignInAsync()
        {
            if (_authManager.Auth == null)
            {
                Log.Warning(Constants.AUTHMANGER_IS_NOT_INITIALIZED);
                return false;
            }
            Log.Info(Constants.SIGN_IN_ANONYMOUSLY_TRYING);
            try
            {
                FirebaseUser user = await _authManager.Auth.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Log.Warning(Constants.SIGN_IN_ANONYMOUSLY_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Info(Constants.SIGN_IN_ANONYMOUSLY_SUCCESS);
            return true;
        }
    }
}