// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano
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