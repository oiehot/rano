namespace Rano.Admob
{
    using System;
    using UnityEngine;
    using GoogleMobileAds.Api;
    using Rano.Core; // Singleton

    public class InterstitialAdManager : Singleton<InterstitialAdManager>
    {
        InterstitialAd ad;

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
            Debug.Log("InterstitialAdManager.Start(): Start");
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
            
            this.LoadAd();
        }
    #endregion

    #region Action
        public void LoadAd()
        {
            Debug.Log("InterstitialAdManager.LoadAd(): Create New InterstitialAd Object");
            ad = new InterstitialAd(adUnitId);
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdClosed += OnAdClosed;
            ad.OnAdLeavingApplication += OnAdLeavingApplication;
                    
            AdRequest request = new AdRequest.Builder().Build();

            // 테스트 기기를 통해 광고 테스트를 하는 경우
            if (deviceId != null)
            {
                request = new AdRequest.Builder()
                    .AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(deviceId)
                    .Build();
            }
            Debug.Log("InterstitialAdManager.LoadAd(): LoadAd Request");
            ad.LoadAd(request);
        }

        public void Show()
        {
            Debug.Log("InterstitialAdManager.Show(): Start");
            if (ad.IsLoaded())
            {
                Debug.Log("InterstitialAdManager.Show(): Call Show");
                ad.Show();
            }
        }
    #endregion

    #region Event
        void OnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("InterstitialAdManager.OnAdLoaded()");
        }

        void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("InterstitialAdManager.OnAdFailedToLoad(): " + args.Message);
        }

        void OnAdOpening(object sender, EventArgs args)
        {
            Debug.Log("InterstitialAdManager.OnAdOpening()");
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            Debug.Log("InterstitialAdManager.OnAdClosed()");
            this.LoadAd();  // 닫히면 바로 다음 광고를 로드한다.
        }

        void OnAdLeavingApplication(object sender, EventArgs args)
        {
            Debug.Log("InterstitialAdManager.OnAdLeavingApplication()");
        }
    #endregion
    }
}
