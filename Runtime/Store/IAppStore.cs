#nullable enable

namespace Rano.Store
{
    public interface IAppStore
    {
        public string GetWebUrl(string bundleId);
        public string GetStoreUrl(string bundleId);
    }
}
