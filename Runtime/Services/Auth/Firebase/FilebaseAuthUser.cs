#nullable enable

using Firebase.Auth;

namespace Rano.Services.Auth
{
    public sealed class FirebaseAuthUser : IAuthUser
    {
        private readonly FirebaseUser _user;
        public string Id => _user.UserId;
        public string DisplayName => _user.DisplayName;
        public bool IsAnonymous => _user.IsAnonymous;
        public bool IsEmailVerified => _user.IsEmailVerified;

        public FirebaseAuthUser(FirebaseUser user)
        {
            _user = user;
        }
    }
}