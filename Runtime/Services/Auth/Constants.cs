namespace Rano.Services.Auth
{
    public static class AuthMessages
    {
        public static string AUTHMANAGER_INIT_FAILED = "AuthManager Initialize Failed";
        public static string AUTHMANGER_IS_NOT_INITIALIZED = "Authentication is not initialized";
        public static string SOCIAL_AUTH_SUCCESS = "SocialAuthentication Success";
        public static string SOCIAL_AUTH_FAILED = "SocialAuthentication Failed";
        public static string SOCIAL_IS_NOT_AUTHENTICATED = "Social is not authenticated";
        public static string CREDENTIAL_CANT_GET = "Can't get credential";
        public static string CREDENTIAL_LINK_SUCCESS = "LinkCredential Success";
        public static string CREDENTIAL_LINK_ERROR = "LinkCredential Error";
        public static string CREDENTIAL_LINK_FAILED_NOT_SIGNED_IN = "LinkCredential Failed (not signed in)";        
        public static string CREDENTIAL_UNLINK_SUCCESS = "UnlinkCredential Success";
        public static string CREDENTIAL_UNLINK_ERROR = "UnlinkCredential Error";
        public static string CREDENTIAL_UNLINK_FAILED_NOT_SIGNED_IN = "UnlinkCredential Failed (not signed in)";
        public static string SIGN_IN_ANONYMOUSLY_TRYING = "SignInAnonymous Trying...";
        public static string SIGN_IN_ANONYMOUSLY_SUCCESS = "SignInAnonymous Success";    
        public static string SIGN_IN_ANONYMOUSLY_ERROR = "SignInAnonymous Error";
        public static string SIGN_IN_WITH_CREDENTIAL_SUCCESS = "SignInWithCredential Success";
        public static string SIGN_IN_WITH_CREDENTIAL_ERROR = "SignInWithCredential Error";
        public static string SIGN_OUT_ERROR = "SignOut Error";
        public static string SIGNED_IN = "Signed In";
        public static string SIGNED_OUT = "Signed Out";
        public static string SIGNED_OUT_ALREADY = "Already signed out (Skip the sign out)";
    }
}