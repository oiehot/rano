// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Rano;

namespace Rano.PlatformServices.Gaming
{
    public enum TimeScope
    {
        Today,
        Week,
        AllTime
    }

    public static class TimeScopeExtensions
    {
        public static VoxelBusters.EssentialKit.LeaderboardTimeScope ToVoxelBusterEnum(this TimeScope timeScope)
        {
            VoxelBusters.EssentialKit.LeaderboardTimeScope ts;
            switch (timeScope)
            {
                case TimeScope.Today:
                    ts = VoxelBusters.EssentialKit.LeaderboardTimeScope.Today;
                    break;
                case TimeScope.Week:
                    ts = VoxelBusters.EssentialKit.LeaderboardTimeScope.Week;
                    break;
                case TimeScope.AllTime:
                    ts = VoxelBusters.EssentialKit.LeaderboardTimeScope.AllTime;
                    break;
                default:
                    throw new Exception();
            }
            return ts;
        }
    }
}