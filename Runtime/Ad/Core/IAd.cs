using System;

namespace Rano.Ad
{
    public interface IAd
    {
        public EAdState State { get; }
        
        public event Action OnAdLoading;
        public event Action OnAdLoaded;
        public event Action OnAdOpening;
        public event Action OnAdClosed;
        public event Action OnAdFailedToLoad;
        public event Action OnAdFailedToShow;
        public event Action<EAdState> OnStateChanged;

        public void ShowAd();

        // public void LoadAd();
        // public void UnloadAd();
    }
}