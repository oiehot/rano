#nullable enable

using System.Threading.Tasks;

namespace Rano.Auth
{
    public interface IAuthModule
    {
        public Task<bool> SignInAsync();
    }
}