using UnityEngine;

namespace Rano.PlatformServices.Gaming
{
    [CreateAssetMenu(fileName = "GpgsSettings", menuName = "Rano/Settings/GPGS Settings")]
    public class GpgsSettingsSO : ScriptableObject
    {
        public string gpgsApplicationId; // ex) *3***1687**
    }
}