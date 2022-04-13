using UnityEngine;

namespace Rano.PlatformServices.Admob
{
    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "Rano/Platform Services/Admob/Admob Settings")]
    public class AdmobSettingsSO : ScriptableObject
    {
        [Header("Admob Application Ids")]
        public string iosId;
        public string androidId; // ex) ca-app-pub-****14***60****5~9048***80*
        public bool delayAppMeasurement;
    }
}