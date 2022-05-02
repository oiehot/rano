namespace Rano.PlatformServices.Admob
{
    public interface IAd
    {
        bool IsLoading { get; }
        bool IsLoaded { get; }
        void LoadAd();
        void ShowAd();
    }
}