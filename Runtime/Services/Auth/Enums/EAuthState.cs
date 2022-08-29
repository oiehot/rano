#nullable enable

namespace Rano.Services.Auth
{   
    public enum EAuthState
    {
        Initializing,
        SignedOut,
        SignInProgress,
        SignedIn,
        SignOutProgress
    }
}