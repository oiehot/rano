using UnityEngine;

namespace Rano.Ad.Admob
{
    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "Rano/Ad/Admob/Admob Settings")]
    public class AdmobSettingsSO : ScriptableObject
    {
        [Header("Admob Application Ids")]
        public string iosId;
        public string androidId;
        public bool delayAppMeasurement;
    }
}