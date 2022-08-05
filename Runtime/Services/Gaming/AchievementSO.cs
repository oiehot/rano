using UnityEngine;

namespace Rano.Services.Gaming
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Rano/Services/Gaming/Achievement")]
    public class AchievementSO : ScriptableObject
    {
        [Header("Ids")]
        /// <summary>
        /// 개발에서 사용되는 Id (유니티)
        /// </summary>
        public string id;
        
        public string iosId;
        public string tvosId;
        public string androidId;
        
        [Header("Informations")]
        public string title;
        public int numberOfStepsToUnlock;
        
        [Header("Build")]
        public bool includeInBuild;
    }
}