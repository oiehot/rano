#nullable enable

using System;

namespace Rano.Database
{
    public class NotFoundDataException : Exception
    {
        public NotFoundDataException()
        {
        }

        public NotFoundDataException(string message)
            : base(message)
        {
        }

        public NotFoundDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}