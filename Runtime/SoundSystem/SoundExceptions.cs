using System;

namespace Rano.SoundSystem
{
    public class NotFoundSoundLayerException : Exception
    {
        public NotFoundSoundLayerException()
        {
        }

        public NotFoundSoundLayerException(string message)
            : base(message)
        {
        }

        public NotFoundSoundLayerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}