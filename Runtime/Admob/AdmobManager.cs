using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Rano; // Logger

namespace Rano.Admob
{
    public class AdmobManager : Singleton<AdmobManager>
    {
        public string appId;
        public void Awake()
        {
            SysLog.Info("Begin");
            MobileAds.Initialize(appId);
        }
    }
}