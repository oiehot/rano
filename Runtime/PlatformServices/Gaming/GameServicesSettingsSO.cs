using UnityEngine;

namespace Rano.PlatformServices.Gaming
{
    [CreateAssetMenu(fileName = "GameServicesSettings", menuName = "Rano/Settings/GameServices Settings")]
    public class GameServicesSettingsSO : ScriptableObject
    {
        [Header("Game Services")]
        public string gpgsApplicationId; // ex) *3***1687**
    }
}