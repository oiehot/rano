using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Rano;
using UnityEngine.Events; // UnityAction

namespace Rano.Admob
{
    public class RewardAdManager: Singleton<RewardAdManager>
    {
        private RewardedAd ad;
        
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
            
            CreateAndLoadAd();
        }

        public void CreateAndLoadAd()
        {
            Log.Info("Create and LoadAd");

            // RewardedAd는 일회용 객체다.
            // 보상형 광고가 표시된 후에는 이 객체를 사용해 다른 광고를 로드할 수 없다.
            // 다른 보상형 광고를 요청하려면 RewardedAd 객체를 만들어야 한다.
            ad = new RewardedAd(adUnitId);
            ad.OnAdLoaded += OnAdLoaded;
            ad.OnAdFailedToLoad += OnAdFailedToLoad;
            ad.OnAdOpening += OnAdOpening;
            ad.OnAdFailedToShow += OnAdFailedToShow;
            ad.OnUserEarnedReward += OnUserEarnedReward;
            ad.OnAdClosed += OnAdClosed;
            // ad.OnAdLeavingApplication += OnAdLeavingApplication;

            AdRequest request;
            if (deviceId == null)
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
                    .AddTestDevice(deviceId)
                    .Build();
            }
            ad.LoadAd(request);
        }

        public void Show()
        {
            Log.Important("Show Ad");
            if (ad.IsLoaded())
            {
                ad.Show();
            }
            else
            {
                Log.Warning("Can't Show Ad. Not loaded");
            }
        }

        /// <summary>광고 로드가 완료될 때 실행</summary>
        void OnAdLoaded(object sender, EventArgs args)
        {
            // Log.Info("Called");
        }

        /// <summary>광고 로드에 실패할 때 실행</summary>
        void OnAdFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Log.Warning(args.Message);
        }

        /// <summary>
        /// 광고가 표시될 때 기기 화면을 덮는다.
        /// 이때 필요한 경우 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋다.
        /// </summary>
        void OnAdOpening(object sender, EventArgs args)
        {
            // Log.Info("Called");
        }

        /// <summary>광고 표시에 실패할 떄 실행된다.</summary>
        void OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Log.Warning(args.Message);
        }

        /// <summary>
        /// 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 광고를 닫을 때 실행된다.
        /// 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리하다.
        /// </summary>
        void OnAdClosed(object sender, EventArgs args)
        {
            // Log.Info("Called");
            this.CreateAndLoadAd(); // 닫히면 바로 다음 광고를 로드한다.
        }

        /// <summary>사용자가 동영상 시청에 대한 보상을 받아야 할 때 실행된다.</summary>
        void OnUserEarnedReward(object sender, Reward reward)
        {
            string type = reward.Type;
            double amount = reward.Amount;
            Log.Important($"Type:{type}, Amount:{amount}"); // ex) Reward, 10
        }
    }
}
