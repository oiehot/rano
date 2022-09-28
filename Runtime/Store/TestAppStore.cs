#nullable enable

namespace Rano.Store
{
    public sealed class TestAppStore : IAppStore
    {
        public string GetWebUrl(string bundleId)
        {
            bundleId = "com.google.android.youtube";
            return $"https://play.google.com/store/apps/details?id={bundleId}";
        }
        
        public string GetStoreUrl(string bundleId)
        {
            bundleId = "com.google.android.youtube";
            return $"https://play.google.com/store/apps/details?id={bundleId}";
        }
    }
}