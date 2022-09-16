namespace Rano.Ad
{
    public enum AdVendor
    {
        None,
        Admob
    }

    public enum AdType
    {
        None,
        AppOpen,
        Banner,
        Interstitial,
        InterstitialMovie,
        Rewarded,
        RewardedInterstitial,
        NativeAdvanced,
        NativeAdvancedMovie
    }
    
    public enum AdState
    {
        None,
        NotLoaded,
        Loading,
        Available,
        Opening,
        Closed,
        Unknown
    }
}