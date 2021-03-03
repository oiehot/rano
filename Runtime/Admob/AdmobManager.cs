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
    public class AdmobManager : Singleton<AdmobManager>
    {
        public string appId;

        public void Start()
        {
            Log.Info("Begin");
            MobileAds.Initialize(appId);
        }
    }
}

#endif