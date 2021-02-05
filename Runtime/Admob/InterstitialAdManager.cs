// TODO: AFO2가 완성된 이후 AFO2의 에드몹 스크립트를 정리하여 이곳에 놓을것
/*
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

        void Awake()
        {
            Log.Info("Begin");
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
        }

        void Start()
        {
            Log.Info("Begin");
            CreateAndLoadAd();
        }

        public void CreateAndLoadAd()
        {
            Log.Info("Begin");
            Log.Info("Create InterstitialAd Object");
            Log.Info($"Using AdUnitId: {adUnitId}");
            ad = new InterstitialAd(adUnitId);
            Log.Info("Connect Event Handlers");
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdClosed += OnAdClosed;
            ad.OnAdLeavingApplication += OnAdLeavingApplication;
                    
            AdRequest request;
            if (deviceId == null)
            {
                // 테스트 기기를 통해 광고 테스트를 하는 경우
                Log.Info("Run Build()");
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
            Log.Info("LoadAd(request) Before");
            ad.LoadAd(request);
            Log.Info("LoadAd(request) After");
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
        }

        void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
        }

        void OnAdOpening(object sender, EventArgs args)
        {
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            this.CreateAndLoadAd();  // 닫히면 바로 다음 광고를 로드한다.
        }

        void OnAdLeavingApplication(object sender, EventArgs args)
        {
        }
    }
}
*/