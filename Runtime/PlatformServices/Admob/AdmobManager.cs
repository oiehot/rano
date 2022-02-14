// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano;
using GoogleMobileAds;
using GoogleMobileAds.Api;

namespace Rano.PlatformServices.Admob
{
    /// <summary>
    /// Admob를 초기화하고 관리하는 클래스.
    /// </summary>
    public sealed class AdmobManager : MonoSingleton<AdmobManager>
    {
        protected override void Awake()
        {
            base.Awake();
            Log.Info($"Admob Initalize Requested");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            SetTestDevices();
#endif
            MobileAds.Initialize(OnInitComplete);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// 프로그래머틱 방식으로 테스트장치 설정.
        /// </summary>
        /// <remarks>
        /// Development 빌드로 앱을 실행하면 나오는 로그에서 장치Id를 얻고 여기에 기재하도록 한다.
        /// 이 데이터는 릴리즈 빌드에 포함되어선 절대 안된다.
        ///
        /// 안드로이드 로그의 예)
        /// I / Ads: Use
        /// RequestConfiguration.Builder
        /// .setTestDeviceIds(Arrays.asList("33BE2250B43518CCDA7DE426D04EE231"))
        /// to get test ads on this device.
        ///
        /// iOS 로그의 예)
        /// <Google> To get test ads on this device, set:
        /// GADMobileAds.sharedInstance.requestConfiguration.testDeviceIdentifiers =
        /// @[ @"2077ef9a63d2b398840261c8221a0c9b" ];
        /// 
        /// https://developers.google.com/admob/unity/test-ads?hl=ko#add_your_test_device_programmatically
        /// </remarks>
        private void SetTestDevices()
        {
            // TIP: 여기에 테스트 디바이스 Id 를 추가할것.
            List<string> items = new List<string>{
                "60170BB4E800B2E49C50C2E22050016E",
                "26DB568E05A345E163257223FB462AB5"
            };

            Log.Info($"Set TestDevices:");
            foreach (var item in items)
            {
                Log.Info($"  {item}");
            }
            RequestConfiguration requestConfiguration = new RequestConfiguration
                .Builder()
                .SetTestDeviceIds(items)
                .build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
        }
#endif

        /// <summary>
        /// 초기화 완료시 호출됨.
        /// </summary>
        /// <param name="status"></param>
        private void OnInitComplete(InitializationStatus status)
        {
            Log.Info($"Admob Initialize Completed");
        }
    }
}