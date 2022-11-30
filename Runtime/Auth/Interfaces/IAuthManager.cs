#nullable enable

using System;

namespace Rano.Auth
{
    public interface IAuthManager
    {
        public string? UserID { get; }
        public bool IsInitialized { get; }
        public bool IsAuthenticated { get; }
        public bool Initialize();
        public bool SignOut();
        public event Action OnSignIn;
        public event Action OnSignOut;
    }
}