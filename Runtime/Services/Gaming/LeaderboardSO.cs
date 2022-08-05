using UnityEngine;

namespace Rano.Services.Gaming
{
    [CreateAssetMenu(fileName = "Leaderboard", menuName = "Rano/Services/Gaming/Leaderboard")]
    public class LeaderboardSO : ScriptableObject
    {
        [Header("Ids")]
        public string id;
        public string iosId;
        public string tvosId;
        public string androidId;
        
        [Header("Informations")]
        public string title;
        
        [Header("Core")]
        public bool includeInBuild;
    }
}