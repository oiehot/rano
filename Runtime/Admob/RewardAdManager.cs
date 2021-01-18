namespace Rano.Admob
{
    using System;
    using UnityEngine;
    using GoogleMobileAds.Api;
    using Rano.Core; // Singleton

    public class RewardAdManager : Singleton<RewardAdManager>
    {
        public RewardedAd ad;

        private string adUnitId;

        public string androidAdUnitId;
        public string androidTestAdUnitId;

        public string iosAdUnitId;
        public string iosTestAdUnitId;

        public string otherAdUnitId;
        public string otherTestAdUnitId;

        public string deviceId;

    #region Setup
        public void Start()
        {
            Debug.Log("RewardAdManager.Start(): Start");
            #if UNITY_ANDROID
                #if DEVELOPMENT_BUILD
                    adUnitId = androidTestAdUnitId;
                #else
                    adUnitId = androidAdUnitId;
                #endif
            #elif UNITY_IOS
                #if DEVELOPMENT_BUILD
                    adUnitId = iosTestAdUnitId;
                #else
                    adUnitId = iosAdUnitId;
                #endif
            #else
                #if DEVELOPMENT_BUILD
                    adUnitId = otherTestAdUnitId;
                #else
                    adUnitId = otherAdUnitId;
                #endif
            #endif

            ad = new RewardedAd(adUnitId);
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdFailedToShow += OnAdFailedToShow;
            ad.OnUserEarnedReward += OnUserEarnedReward;
            ad.OnAdClosed += OnAdClosed;
            // ad.OnAdLeavingApplication += OnAdLeavingApplication;
            LoadAd();
        }
    #endregion

    #region Body
        public void LoadAd()
        {
            AdRequest request;
            request = new AdRequest.Builder().Build();

            // 테스트 기기를 통해 광고 테스트를 하는 경우
            if (deviceId != null)
            {
                request = new AdRequest.Builder()
                    .AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(deviceId)
                    .Build();
            }

            Debug.Log("RewardAdManager.LoadAd(): LoadAD Requested");
            ad.LoadAd(request);
        }

        public void Show()
        {
            Debug.Log("RewardAdManager.Show(): Requested");
            if (ad.IsLoaded())
            {
                Debug.Log("RewardAdManager.Show(): Call Show");
                ad.Show();
            }
            else
            {
                Debug.Log("RewardAdManager.Show(): Not Loaded. Call LoadAd");
                this.LoadAd();
            }
        }
    #endregion

    #region Handlers
        void OnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("RewardAdManager.OnAdLoaded()");
        }

        void OnAdFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Debug.Log("RewardAdManager.OnAdFailedToLoad(): " + args.Message);
        }

        void OnAdOpening(object sender, EventArgs args)
        {
            Debug.Log("RewardAdManager.OnAdOpening()");
        }

        void OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.Log("RewardAdManager.OnAdFailedToShow(): " + args.Message);
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            Debug.Log("RewardAdManager.OnAdClosed()");
            this.LoadAd(); // 닫히면 바로 다음 광고를 로드한다.
        }

        void OnUserEarnedReward(object sender, Reward reward)
        {
            #if DEVELOPMENT_BUILD
                string type = reward.Type;
                double amount = reward.Amount;
                Debug.Log("RewardAdManager.OnUserEarnedReward(): " + reward.ToString() + " " + type); // 1 Bomb
            #endif
        }
    #endregion
    }
}
