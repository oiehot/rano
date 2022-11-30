using System;

namespace Rano.Ad
{
    public abstract class RewardedAd : Ad
    {
        public Action<int, string> OnAdReward;
    }
}