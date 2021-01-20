using System;
using UnityEngine;
using GoogleMobileAds.Api;
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

        public void Awake()
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

        public void LoadAd()
        {
            Log.Info("Create New InterstitialAd Object");
            ad = new InterstitialAd(adUnitId);
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdClosed += OnAdClosed;
            ad.OnAdLeavingApplication += OnAdLeavingApplication;
                    
            AdRequest request;
            if (deviceId == null)
            {
                // 테스트 기기를 통해 광고 테스트를 하는 경우
                request = new AdRequest.Builder().Build();
            }
            else
            {
                // 테스트 기기 사용시
                Log.Important("Using Test Device");
                request = new AdRequest.Builder()
                    .AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(deviceId)
                    .Build();
            }
            ad.LoadAd(request);
        }

        public void Show()
        {
            if (ad.IsLoaded())
            {
                Log.Important("Show InterstitialAd");
                ad.Show();
            }
            else
            {
                Log.Warning("Can't Show InterstitialAd. Not Loaded");
            }
        }

        void OnAdLoaded(object sender, EventArgs args)
        {
            // Log.Info("Called");
        }

        void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Log.Info(args.Message);
        }

        void OnAdOpening(object sender, EventArgs args)
        {
            // Log.Info("Called");
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            // Log.Info("Called");
            this.LoadAd();  // 닫히면 바로 다음 광고를 로드한다.
        }

        void OnAdLeavingApplication(object sender, EventArgs args)
        {
            // Log.Info("Called");
        }
    }
}
