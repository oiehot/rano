namespace Rano.Auth.Firebase
{
    public static class Constants
    {
        public const string AUTHMANAGER_INIT_FAILED = "AuthManager Initialize Failed";
        public const string AUTHMANGER_IS_NOT_INITIALIZED = "Authentication is not initialized";
        public const string SOCIAL_AUTH_SUCCESS = "SocialAuthentication Success";
        public const string CREDENTIAL_LINK_SUCCESS = "LinkCredential Success";
        public const string CREDENTIAL_LINK_ERROR = "LinkCredential Error";
        public const string CREDENTIAL_LINK_FAILED_NOT_SIGNED_IN = "LinkCredential Failed (not signed in)";        
        public const string CREDENTIAL_UNLINK_SUCCESS = "UnlinkCredential Success";
        public const string CREDENTIAL_UNLINK_ERROR = "UnlinkCredential Error";
        public const string CREDENTIAL_UNLINK_FAILED_NOT_SIGNED_IN = "UnlinkCredential Failed (not signed in)";
        public const string SIGN_IN_WITH_CREDENTIAL_SUCCESS = "SignInWithCredential Success";
        public const string SIGN_IN_WITH_CREDENTIAL_ERROR = "SignInWithCredential Error";
        public const string SIGN_OUT_ERROR = "SignOut Error";
        public const string SIGNED_IN = "Signed In";
        public const string SIGNED_OUT = "Signed Out";
        public const string SIGNED_OUT_ALREADY = "Already signed out (Skip the sign out)";
    }
}