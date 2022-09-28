#nullable enable

namespace Rano.Store
{
    public sealed class AppleAppStore : IAppStore
    {
        public string GetWebUrl(string bundleId)
        {
            return $"http://itunes.apple.com/lookup?bundleId={bundleId}";
        }
        
        public string GetStoreUrl(string bundleId)
        {
            return $"itms-apps://itunes.apple.com/app/id{bundleId}";
        }
    }
}