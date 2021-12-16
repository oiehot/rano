// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

// TODO: 에드몹 테스트 디바이스의 처리
//#define ADMOB_TEST_DEVICE

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
        void Start()
        {
            Log.Info($"에드몹 초기화 요청.");
#if ADMOB_TEST_DEVICE
            string testDeviceId = null;
            if (testDeviceId != null)
            {
                Log.Info($"테스트 디바이스 지정 ({testDeviceId})");
                List<string> testDevices = new List<string>();
                testDevices.Add(testDeviceId);
                RequestConfiguration requestConfiguration = new RequestConfiguration
                    .Builder()
                    .SetTestDeviceIds(testDevices)
                    .build();
                MobileAds.SetRequestConfiguration(requestConfiguration);
            }
            else
            {
                throw new Exception("테스트 디바이스 Id가 없음.");
            }
#endif
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