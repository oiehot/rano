#if ADMOB

namespace Rano.Admob
{
    using System;
    using UnityEngine;
    using GoogleMobileAds.Api;
    using Rano.Core; // Singleton

    public class AdManager : Singleton<AdManager>
    {
        public string appId;
        
        public void Awake()
        {
            MobileAds.Initialize(appId);
        }
    }
}

#endif