using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Rano.PlatformServices.Admob
{
    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "Rano/Settings/Admob Settings")]
    public class AdmobSettingsSO : ScriptableObject
    {
        public string iosId;
        public string androidId; // ex) ca-app-pub-****14***60****5~9048***80*
        public bool delayAppMeasurement;
    }
}