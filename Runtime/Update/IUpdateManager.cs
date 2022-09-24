#nullable enable

using System.Threading.Tasks;

namespace Rano.Update
{   
    public interface IUpdateManager
    {
        public bool IsInitialized { get; }
        public void Initialize();
        public Task<UpdateInfo> GetUpdateInfoAsync();
        public Task<bool> UpdateAsync();
    }
}