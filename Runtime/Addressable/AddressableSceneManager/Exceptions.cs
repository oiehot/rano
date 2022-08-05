using System;

namespace Rano.Addressable
{
    public class AddressableSceneManagerException : Exception
    {
        public AddressableSceneManagerException()
        {
        }

        public AddressableSceneManagerException(string message) : base(message)
        {
        }

        public AddressableSceneManagerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}