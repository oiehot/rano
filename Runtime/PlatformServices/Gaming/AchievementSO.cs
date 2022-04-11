using UnityEngine;

namespace Rano.PlatformServices.Gaming
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Rano/Platform Services/Gaming/Achievement")]
    public class AchievementSO : ScriptableObject
    {
        [Header("Ids")]
        public string id;
        public string iosId;
        public string tvosId;
        public string androidId;
        
        [Header("Informations")]
        public string title;
        public int numberOfStepsToUnlock;
    }
}