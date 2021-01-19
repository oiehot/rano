using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Rano.Core; // Singleton
using Rano;

namespace Rano.Admob
{
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
            Log.Info("Create New InterstitialAd Object");
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
            Log.Info("LoadAd Request");
            ad.LoadAd(request);
        }

        public void Show()
        {
            Log.Info("Start");
            if (ad.IsLoaded())
            {
                Log.Info("Call Show");
                ad.Show();
            }
        }
    #endregion

    #region Event
        void OnAdLoaded(object sender, EventArgs args)
        {
            Log.Info("Called");
        }

        void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Log.Info(args.Message);
        }

        void OnAdOpening(object sender, EventArgs args)
        {
            Log.Info("Called");
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            Log.Info("Called");
            this.LoadAd();  // 닫히면 바로 다음 광고를 로드한다.
        }

        void OnAdLeavingApplication(object sender, EventArgs args)
        {
            Log.Info("Called");
        }
    #endregion
    }
}
