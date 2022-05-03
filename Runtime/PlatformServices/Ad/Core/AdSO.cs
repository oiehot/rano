using System;
using UnityEngine;

namespace Rano.PlatformServices.Ad
{
    [CreateAssetMenu(fileName = "AdmobAd", menuName = "Rano/Platform Services/Admob/Admob Ad")]
    public class AdSO : ScriptableObject
    {
        public AdVendor vendor;
        public AdType type;
        public string adName;
        public string iosUnitId;
        public string androidUnitId;

        public string UnitId 
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                bool developmentBuild = Debug.isDebugBuild;

                string adUnitId = (platform, type, developmentBuild) switch
                {
                    // iOS or Android, Release
                    (RuntimePlatform.IPhonePlayer, _, false) => iosUnitId,
                    (RuntimePlatform.Android, _, false) => androidUnitId,

                    // iOS, DevelopmentBuild
                    (RuntimePlatform.IPhonePlayer, AdType.AppOpen, true) => "ca-app-pub-3940256099942544/5662855259",
                    (RuntimePlatform.IPhonePlayer, AdType.Banner, true) => "ca-app-pub-3940256099942544/2934735716",
                    (RuntimePlatform.IPhonePlayer, AdType.Interstitial, true) => "ca-app-pub-3940256099942544/4411468910",
                    (RuntimePlatform.IPhonePlayer, AdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/5135589807",
                    (RuntimePlatform.IPhonePlayer, AdType.Rewarded, true) => "ca-app-pub-3940256099942544/1712485313",
                    (RuntimePlatform.IPhonePlayer, AdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/6978759866",
                    (RuntimePlatform.IPhonePlayer, AdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/3986624511",
                    (RuntimePlatform.IPhonePlayer, AdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/2521693316",

                    // Android, DevelopmentBuild
                    (RuntimePlatform.Android, AdType.AppOpen, true) => "ca-app-pub-3940256099942544/3419835294",
                    (RuntimePlatform.Android, AdType.Banner, true) => "ca-app-pub-3940256099942544/6300978111",
                    (RuntimePlatform.Android, AdType.Interstitial, true) => "ca-app-pub-3940256099942544/1033173712",
                    (RuntimePlatform.Android, AdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/8691691433",
                    (RuntimePlatform.Android, AdType.Rewarded, true) => "ca-app-pub-3940256099942544/5224354917",
                    (RuntimePlatform.Android, AdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/5354046379",
                    (RuntimePlatform.Android, AdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/2247696110",
                    (RuntimePlatform.Android, AdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/1044960115",

                    // Etc, DevelopmentBuild (==Android)
                    (_, AdType.AppOpen, true) => "ca-app-pub-3940256099942544/3419835294",
                    (_, AdType.Banner, true) => "ca-app-pub-3940256099942544/6300978111",
                    (_, AdType.Interstitial, true) => "ca-app-pub-3940256099942544/1033173712",
                    (_, AdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/8691691433",
                    (_, AdType.Rewarded, true) => "ca-app-pub-3940256099942544/5224354917",
                    (_, AdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/5354046379",
                    (_, AdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/2247696110",
                    (_, AdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/1044960115",
                    
                    // Default
                    (_, _, _) => null
                };

                if (String.IsNullOrEmpty(adUnitId))
                {
                    throw new Exception($"{adName} - 광고id가 없음 (platform:{platform}, developmentBuild:{developmentBuild}");
                }

                return adUnitId;
            }
        }
    }
}
