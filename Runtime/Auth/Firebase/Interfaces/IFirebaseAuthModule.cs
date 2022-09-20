#nullable enable

using System.Threading.Tasks;

namespace Rano.Auth.Firebase
{
    public interface IFirebaseAuthModule
    {
        public void Initialize(FirebaseAuthManager authManager);
        public Task<bool> SignInAsync();
    }
}