// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if ADMOB

using System;
using UnityEngine;
using UnityEngine.Events; // UnityAction
using Rano;
using GoogleMobileAds;
using GoogleMobileAds.Api;

namespace Rano.Admob
{
    /// <summary>
    /// 이벤트 핸들러는 Admob 스레드에서 실행되므로 핸들러 안에서는 유니티 엔진 라이브러리를 사용해서는 안된다.
    /// 우리는 이 문제를 핸들러에서는 플래그를 세우고 메인스레드에서 작동되는 Update에서 사용자 이벤트 함수를 호출하는 식으로 해결했다.
    /// 다음 글을 참고할것: https://ads-developers.googleblog.com/2016/04/handling-android-ad-events-in-unity.html
    /// </summary>    
    public class RewardedAdController : MonoBehaviour
    {
        RewardedAd ad;

        string adUnitId;
        public string androidAdUnitId;
        public string androidTestAdUnitId;
        public string iosAdUnitId;
        public string iosTestAdUnitId;
        public string otherAdUnitId;
        public string otherTestAdUnitId;
        public string deviceId;
        
        // 여기에 연결된 함수들은 메인 스레드에서 실행된다.
        public UnityAction OnRewardAdLoaded;
        public UnityAction OnRewardAdOpening;
        public UnityAction OnRewardAdClosed;
        public UnityAction OnRewardAdFailedToLoad;
        public UnityAction OnRewardAdFailedToShow;
        public UnityAction<int> OnRewardGot;        

        private bool onAdLoaded = false;
        private bool onAdOpening = false;
        private bool onAdClosed = false;
        private bool onAdFailedToLoad = false;
        private bool onAdFailedToShow = false;
        private bool onRewardGot = false;
        private int  onRewardGotAmount;

        /// <summary>
        /// 🌟 컴포넌트 시작시 자동으로 광고를 로드하지 않는다.
        /// </summary>
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
        }

        /// <<summary>
        /// 메인 스레드에서 사용자가 연결한 함수들을 실행한다.
        /// </summary>
        void Update()
        {
            if (this.onAdLoaded)
            {
                this.onAdLoaded = false;
                if (this.OnRewardAdLoaded != null) this.OnRewardAdLoaded();
            }
            if (this.onAdOpening)
            {
                this.onAdOpening = false;
                SoundManager.Instance.Pause(); // 광고 출력전 사운드 잠시 정지
                if (this.OnRewardAdOpening != null) this.OnRewardAdOpening();
            }
            if (this.onAdClosed)
            {
                this.onAdClosed = false;
                if (this.OnRewardAdClosed != null) this.OnRewardAdClosed();
                this.ad = null;
                this.CreateAndLoadAd(); // 닫히면 바로 다음 광고를 로드한다.
                SoundManager.Instance.Resume(); // 광고 닫히면 사운드 다시 재개
            }
            if (this.onAdFailedToLoad)
            {
                this.onAdFailedToLoad = false;
                if (this.OnRewardAdFailedToLoad != null) this.OnRewardAdFailedToLoad();
            }
            if (this.onAdFailedToShow)
            {
                this.onAdFailedToShow = false;
                if (this.OnRewardAdFailedToShow != null) this.OnRewardAdFailedToShow();
            }
            if (this.onRewardGot)
            {
                this.onRewardGot = false;
                if (this.OnRewardGot != null) this.OnRewardGot(this.onRewardGotAmount);
                this.onRewardGotAmount = 0;  
            }            
        }

        public void CreateAndLoadAd()
        {
            Log.Info("Begin");

            if (this.IsAdLoaded())
            {
                Log.Warning("The advertisement is already loaded.");
                return;
            }
            
            // RewardedAd는 일회용 객체다.
            // 보상형 광고가 표시된 후에는 이 객체를 사용해 다른 광고를 로드할 수 없다.
            // 다른 보상형 광고를 요청하려면 RewardedAd 객체를 만들어야 한다.
            this.ad = new RewardedAd(this.adUnitId);
            this.ad.OnAdLoaded += this.OnAdLoaded;
            this.ad.OnAdFailedToLoad += this.OnAdFailedToLoad;
            this.ad.OnAdOpening += this.OnAdOpening;
            this.ad.OnAdFailedToShow += this.OnAdFailedToShow;
            this.ad.OnUserEarnedReward += this.OnUserEarnedReward;
            this.ad.OnAdClosed += this.OnAdClosed;
            // ad.OnAdLeavingApplication += this.OnAdLeavingApplication;

            AdRequest request;
            if (this.deviceId == null)
            {
                // 테스트 기기가 아닌 경우
                request = new AdRequest.Builder().Build();
            }
            else
            {
                // 테스트 기기를 통해 광고 테스트를 하는 경우
                Log.Important("Using Test Device");
                request = new AdRequest.Builder()
                    .AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(this.deviceId)
                    .Build();
            }
            this.ad.LoadAd(request);
        }

        public bool IsAdLoaded()
        {
            if (ad != null)
            {
                return this.ad.IsLoaded();
            }
            else
            {
                return false;
            }
        }

        public void Show()
        {
            Log.Important("Begin");
            if (this.ad.IsLoaded())
            {
                this.ad.Show();
            }
            else
            {
                Log.Warning("Can't Show Ad. Not loaded");
                this.OnAdFailedToShow(null, null);
            }
        }

        /// <summary>광고 로드가 완료될 때 실행</summary>
        void OnAdLoaded(object sender, EventArgs args)
        {
            Log.Info("OnAdLoaded");
            this.onAdLoaded = true;
        }

        /// <summary>광고 로드에 실패할 때 실행</summary>
        void OnAdFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Log.Warning("OnAdFailedToLoad");
            this.onAdFailedToLoad = true;
        }

        /// <summary>
        /// 광고가 표시될 때 기기 화면을 덮는다.
        /// 이때 필요한 경우 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋다.
        /// </summary>
        void OnAdOpening(object sender, EventArgs args)
        {
            Log.Info("OnAdOpening");
            this.onAdOpening = true;
        }

        /// <summary>광고 표시에 실패할 때 실행된다.</summary>
        void OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Log.Warning("OnAdFailedToShow");
            this.onAdFailedToShow = true;
        }

        /// <summary>
        /// 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 광고를 닫을 때 실행된다.
        /// 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리하다.
        /// </summary>
        void OnAdClosed(object sender, EventArgs args)
        {
            Log.Info("OnAdClosed");
            this.onAdClosed = true;
        }

        /// <summary>
        /// 보상형 광고를 완수했을 때 호출된다.
        /// </summary>
        void OnUserEarnedReward(object sender, Reward reward)
        {
            Log.Important("OnUserEarnedReward");
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
                this.onRewardGotAmount = (int)reward.Amount / 10; // 테스트 유닛 아이디에서는 10조각을 받는다. 따라서 10으로 나눠서 하나만 받도록 수정한다.
            #else
                this.onRewardGotAmount = (int)reward.Amount;
            #endif
            this.onRewardGot = true;
        }
    }
}

#endif