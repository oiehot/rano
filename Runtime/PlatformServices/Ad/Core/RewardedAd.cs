using System;

namespace Rano.PlatformServices.Ad
{
    public abstract class RewardedAd : Ad
    {
        public Action<int, string> OnAdReward { get; set;  }
    }
}