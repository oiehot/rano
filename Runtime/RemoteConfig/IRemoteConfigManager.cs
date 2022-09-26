#nullable enable

using System.Threading.Tasks;
using System.Collections.Generic;

namespace Rano.RemoteConfig
{
    public interface IRemoteConfigManager
    {
        public bool IsInitialized { get; }
        public Task Initialize(Dictionary<string,object>? defaults=null);
        public Task<bool> FetchAsync();
        public Task<bool> ActivateAsync();
        public bool TryGetString(string key, out string? value);
        public bool TryGetDouble(string key, out double value);
        public bool TryGetLong(string key, out long value);
        public bool TryGetBool(string key, out bool value);
        public void LogStatus();
    }
}