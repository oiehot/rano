using System;

namespace Rano.Services.Ad
{
    public abstract class RewardedAd : Ad
    {
        public Action<int, string> OnAdReward { get; set;  }
    }
}