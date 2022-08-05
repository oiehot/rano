using UnityEngine;

namespace Rano.App
{
    [CreateAssetMenu(fileName = "AppSettings", menuName = "Rano/Settings/App Settings")]
    public class AppSettingsSO : ScriptableObject
    {
        [Header("Product Ids")]
        public string defaultId;
        public string iosId;
        public string androidId;
        public string tvosId;
        public string macId;
    }
}