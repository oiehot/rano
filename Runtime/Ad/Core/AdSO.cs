using System;
using UnityEngine;

namespace Rano.Ad
{
    [CreateAssetMenu(fileName = "AdmobAd", menuName = "Rano/Platform Services/Admob/Admob Ad")]
    public class AdSO : ScriptableObject
    {
        public EAdVendor vendor;
        public EAdType type;
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
                    (RuntimePlatform.IPhonePlayer, EAdType.AppOpen, true) => "ca-app-pub-3940256099942544/5662855259",
                    (RuntimePlatform.IPhonePlayer, EAdType.Banner, true) => "ca-app-pub-3940256099942544/2934735716",
                    (RuntimePlatform.IPhonePlayer, EAdType.Interstitial, true) => "ca-app-pub-3940256099942544/4411468910",
                    (RuntimePlatform.IPhonePlayer, EAdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/5135589807",
                    (RuntimePlatform.IPhonePlayer, EAdType.Rewarded, true) => "ca-app-pub-3940256099942544/1712485313",
                    (RuntimePlatform.IPhonePlayer, EAdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/6978759866",
                    (RuntimePlatform.IPhonePlayer, EAdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/3986624511",
                    (RuntimePlatform.IPhonePlayer, EAdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/2521693316",

                    // Android, DevelopmentBuild
                    (RuntimePlatform.Android, EAdType.AppOpen, true) => "ca-app-pub-3940256099942544/3419835294",
                    (RuntimePlatform.Android, EAdType.Banner, true) => "ca-app-pub-3940256099942544/6300978111",
                    (RuntimePlatform.Android, EAdType.Interstitial, true) => "ca-app-pub-3940256099942544/1033173712",
                    (RuntimePlatform.Android, EAdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/8691691433",
                    (RuntimePlatform.Android, EAdType.Rewarded, true) => "ca-app-pub-3940256099942544/5224354917",
                    (RuntimePlatform.Android, EAdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/5354046379",
                    (RuntimePlatform.Android, EAdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/2247696110",
                    (RuntimePlatform.Android, EAdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/1044960115",

                    // Etc, DevelopmentBuild (==Android)
                    (_, EAdType.AppOpen, true) => "ca-app-pub-3940256099942544/3419835294",
                    (_, EAdType.Banner, true) => "ca-app-pub-3940256099942544/6300978111",
                    (_, EAdType.Interstitial, true) => "ca-app-pub-3940256099942544/1033173712",
                    (_, EAdType.InterstitialMovie, true) => "ca-app-pub-3940256099942544/8691691433",
                    (_, EAdType.Rewarded, true) => "ca-app-pub-3940256099942544/5224354917",
                    (_, EAdType.RewardedInterstitial, true) => "ca-app-pub-3940256099942544/5354046379",
                    (_, EAdType.NativeAdvanced, true) => "ca-app-pub-3940256099942544/2247696110",
                    (_, EAdType.NativeAdvancedMovie, true) => "ca-app-pub-3940256099942544/1044960115",
                    
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
