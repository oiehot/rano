using UnityEngine;

namespace Rano.PlatformServices.Ad.Admob
{
    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "Rano/Platform Services/Admob/Admob Settings")]
    public class AdmobSettingsSO : ScriptableObject
    {
        [Header("Admob Application Ids")]
        public string iosId;
        public string androidId;
        public bool delayAppMeasurement;
    }
}