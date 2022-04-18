using System;
using UnityEngine;

namespace Rano.PlatformServices.Gaming
{
    public class Achievement
    {
        /// <summary>
        /// 개발에서 사용되는 Id (유니티)
        /// </summary>
        public string id;
        
        /// <summary>
        /// 플랫폼에 입력된 Id (iOS, Android)
        /// </summary>
        public string platformId;
        
        public DateTime lastReportedDate;
        public bool completed;
        public double percentage;
    }

    public class AchievementInfo
    {
        /// <summary>
        /// 개발에서 사용되는 Id (유니티)
        /// </summary>        
        public string id;
        
        /// <summary>
        /// 플랫폼에 입력된 Id (iOS, Android)
        /// </summary>
        public string platformId;
        public string title;
        public string unachievedDescription;
        public string achievedDescription;
        public long maximumPoints;
        public int totalSteps;
        public bool hidden;
        public bool replayable;
    }
}