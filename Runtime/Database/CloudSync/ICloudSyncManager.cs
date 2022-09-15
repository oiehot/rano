#nullable enable

using System.Threading.Tasks;
using Rano.Auth;

namespace Rano.Database.CloudSync
{
    public interface ICloudSyncManager
    {
        public bool IsInitialized { get; }
        public bool Initialize(ILocalDatabase localDatabase, ICloudDatabase cloudDatabase, IAuthManager authManager);
        public Task<bool> SyncAsync();
        public Task<bool> SyncLocalToCloudAsync();
    }
}