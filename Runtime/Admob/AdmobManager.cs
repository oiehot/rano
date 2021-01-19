using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Rano; // Logger
using Rano.Core; // Singleton

namespace Rano.Admob
{
    public class AdmobManager : Singleton<AdmobManager>
    {
        public string appId;
        public void Awake()
        {
            Log.Info("Initialize");
            MobileAds.Initialize(appId);
        }
    }
}