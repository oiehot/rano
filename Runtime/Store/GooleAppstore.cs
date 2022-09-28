#nullable enable

namespace Rano.Store
{
    public sealed class GoogleAppStore : IAppStore
    {
        public string GetWebUrl(string bundleId)
        {
            return $"https://play.google.com/store/apps/details?id={bundleId}";
        }
        
        public string GetStoreUrl(string bundleId)
        {
            return $"market://details?id={bundleId}";
        }
    }
}