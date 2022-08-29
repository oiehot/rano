#nullable enable

namespace Rano.Services.Auth
{
    public interface IAuthUser
    {
        public string Id { get; }
        public string DisplayName { get; }
        public bool IsAnonymous { get; }
        public bool IsEmailVerified { get; }
    }
}