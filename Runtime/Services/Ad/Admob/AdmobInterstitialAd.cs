using GoogleMobileAds.Api;

namespace Rano.Services.Ad.Admob
{
    public sealed class AdmobInterstitialAd : Ad
    {
        private InterstitialAd _ad;
        
        protected override void LoadAdClient()
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
