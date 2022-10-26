using GoogleMobileAds.Api;

namespace Rano.Ad.Admob
{
    public sealed class AdmobInterstitialAd : Ad
    {
        private InterstitialAd _ad;
        
        protected override void LoadAdInternal()
        {
            _ad = new InterstitialAd(AdUnitId);
            _ad.OnAdLoaded += HandleAdLoaded;
            _ad.OnAdFailedToLoad += HandleAdFailedToLoad;
            _ad.OnAdOpening += HandleAdOpening;
            _ad.OnAdFailedToShow += HandleAdFailedToShow;
            _ad.OnAdClosed += HandleAdClosed;
            
            AdRequest request = new AdRequest.Builder().Build();
            _ad.LoadAd(request);
        }

        protected override void UnloadAdInternal()
        {
            if (_ad == null)
            {
                _ad.Destroy();
                _ad = null;
            }
        }

        protected override void ShowAdInternal()
        {
            _ad.Show();
        }
    }
}
