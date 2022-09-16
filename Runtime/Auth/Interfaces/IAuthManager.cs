#nullable enable

using System.Threading.Tasks;

namespace Rano.Auth
{
    public interface IAuthManager
    {
        public string? UserId { get; }
        public bool IsInitialized { get; }
        public bool IsAuthenticated { get; }
        public bool Initialize();
        public bool SignOut();
    }
}