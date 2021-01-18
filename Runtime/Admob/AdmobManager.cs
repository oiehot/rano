namespace Rano.Admob
{
    using System;
    using UnityEngine;
    using GoogleMobileAds.Api;
    using Rano.Core; // Singleton

    public class AdmobManager : Singleton<AdmobManager>
    {
        public string appId;
        
        public void Awake()
        {
            MobileAds.Initialize(appId);
        }
    }
}