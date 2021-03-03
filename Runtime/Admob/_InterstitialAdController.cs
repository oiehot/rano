// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if ADMOB

using System;
using UnityEngine;
using Rano;
using GoogleMobileAds;
using GoogleMobileAds.Api;

namespace Rano.Admob
{
    /// <summary>
    /// 이벤트 핸들러는 Admob 스레드에서 실행되므로 핸들러 안에서는 유니티 엔진 라이브러리를 사용해서는 안된다.
    /// 이 문제를 회피하기 위해서 다음 글을 참고할것: https://ads-developers.googleblog.com/2016/04/handling-android-ad-events-in-unity.html
    /// </summary>    
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

        void Start()
        {
            Log.Info("Begin");

            // 플랫폼에 따라 AdUnitID를 결정한다.
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

            // TODO: 최초 광고 로드 시점을 결정해야합니다.
            CreateAndLoadAd();
        }

        public void CreateAndLoadAd()
        {
            Log.Info("Begin");
            ad = new InterstitialAd(adUnitId);
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdClosed += OnAdClosed;
            // ad.OnAdLeavingApplication += OnAdLeavingApplication;
                    
            AdRequest request;
            if (deviceId == null)
            {
                // 테스트 기기를 통해 광고 테스트를 하는 경우
                request = new AdRequest.Builder().Build();
            }
            else
            {
                // 테스트 기기 사용시
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
            Log.Info("OnAdLoaded");
        }

        void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Log.Warning("OnAdFailedToLoad");
        }

        void OnAdOpening(object sender, EventArgs args)
        {
            Log.Info("OnAdOpening");
        }

        void OnAdClosed(object sender, EventArgs args)
        {
            Log.Info("OnAdClosed");
            this.CreateAndLoadAd();  // 닫히면 바로 다음 광고를 로드한다.
        }

        // void OnAdLeavingApplication(object sender, EventArgs args)
        // {
        // }
    }
}

#endif