#nullable enable

namespace Rano.Ad
{
    public enum EAdVendor
    {
        None,
        Admob
    }

    public enum EAdType
    {
        AppOpen,
        Banner,
        Interstitial,
        InterstitialMovie,
        Rewarded,
        RewardedInterstitial,
        NativeAdvanced,
        NativeAdvancedMovie
    }
    
    public enum EAdState
    {
        NotLoaded,
        Loading,
        Loaded,
        Opening,
        Closed,
        LoadFailed,
        ShowFailed
    }
}