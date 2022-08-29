#if false

#nullable enable

using System.Threading.Tasks;

namespace Rano.Services.Auth
{   
    public interface IAuthManager
    {
        public IAuthUser? User { get; }

        public void CreateUserWithEmailAsync(string email, string password);
        public Task SignInAnonymousAync();
        public void SignInWithEmailAsync(string email, string password);
        public void SignOut();
        public bool IsInitializing { get; }
        public void LogStatus();
    }
}

#endif