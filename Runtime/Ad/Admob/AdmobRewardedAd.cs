using System;
using GoogleMobileAds.Api;

namespace Rano.Ad.Admob
{
    public sealed class AdmobRewardedAd : RewardedAd
    {
        private GoogleMobileAds.Api.RewardedAd _ad;
        private bool _adRewardFlag;
        private int _adRewardAmount;
        private string _adRewardUnit;
        
        protected override void Update()
        {
            base.Update();
            
            if (_adRewardFlag)
            {
                _adRewardFlag = false;
                Log.Info($"{AdName} - 광고 보상받음 ({_adRewardAmount} {_adRewardUnit})");
                OnAdReward?.Invoke(_adRewardAmount, _adRewardUnit);
                _adRewardAmount = 0;
                _adRewardUnit = null;
            }
        }
        
        /// <summary>
        /// 보상형 광고를 완수했을 때 호출된다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleUserEarnedReward(object sender, Reward reward)
        {
            int rewardAmount = 0;
            string rewardUnit = reward.Type;
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                rewardAmount = (int)reward.Amount / 10; // 테스트 유닛 아이디에서는 10조각을 받는다. 따라서 10으로 나눠서 하나만 받도록 수정한다.
#else
                rewardAmount = (int)reward.Amount;
#endif
            if (rewardAmount <= 0 )
            {
                throw new Exception($"{AdName} - 보상 개수가 1미만일 수가 없음.");
            }

            lock (LockObject)
            {
                _adRewardAmount = rewardAmount;
                _adRewardUnit = rewardUnit;
                _adRewardFlag = true;
            }
        }

        protected override void LoadAdClient()
        {
            // RewardedAd는 일회용 객체다.
            // 보상형 광고가 표시된 후에는 이 객체를 사용해 다른 광고를 로드할 수 없다.
            // 다른 보상형 광고를 요청하려면 RewardedAd 객체를 만들어야 한다.
            _ad = new GoogleMobileAds.Api.RewardedAd(AdUnitId);
            _ad.OnAdLoaded += HandleAdLoaded;
            _ad.OnAdFailedToLoad += HandleAdFailedToLoad;
            _ad.OnAdOpening += HandleAdOpening;
            _ad.OnAdFailedToShow += HandleAdFailedToShow;
            _ad.OnUserEarnedReward += HandleUserEarnedReward;
            _ad.OnAdClosed += HandleAdClosed;
            
            AdRequest request = new AdRequest.Builder().Build();
            _ad.LoadAd(request);
        }

        protected override void UnloadAdClient()
        {
            if (_ad == null)
            {
                _ad.Destroy();
                _ad = null;
            }
        }

        protected override void ShowClientAd()
        {
            _ad.Show();
        }
    }
}