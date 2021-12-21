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
        private string _testDeviceId
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            = "60170BB4E800B2E49C50C2E22050016E";
#else
            = null;
#endif

        void Start()
        {
            Log.Info($"에드몹 초기화 요청.");
            if (_testDeviceId != null)
            {
                Log.Info($"테스트 디바이스 지정 ({_testDeviceId})");
                List<string> testDevices = new List<string>();
                testDevices.Add(_testDeviceId);
                RequestConfiguration requestConfiguration = new RequestConfiguration
                    .Builder()
                    .SetTestDeviceIds(testDevices)
                    .build();
                MobileAds.SetRequestConfiguration(requestConfiguration);
            }
            else
            {
#if DEVELOPMENT_BUILD
                Log.Warning("테스트 디바이스 Id가 지정되어있지 않음");
#endif
            }

            MobileAds.Initialize(OnInitComplete);
        }

        /// <summary>
        /// 초기화 완료시 호출됨.
        /// </summary>
        /// <param name="status"></param>
        private void OnInitComplete(InitializationStatus status)
        {
            Log.Info($"에드몹 초기화 완료.");
        }
    }
}