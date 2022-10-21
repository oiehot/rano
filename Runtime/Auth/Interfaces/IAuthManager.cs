#nullable enable

using System;
using System.Threading.Tasks;

namespace Rano.Auth
{
    public interface IAuthManager
    {
        public string? UserID { get; }
        public bool IsInitialized { get; }
        public bool IsAuthenticated { get; }
        public bool Initialize();
        public bool SignOut();

        public Action OnSignIn { get; set; }
        public Action OnSignOut { get; set; }
    }
}