// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano.SaveSystem
{
    public class StateValidateException : Exception
    {
        public StateValidateException()
        {
        }

        public StateValidateException(string message)
            : base(message)
        {
        }

        public StateValidateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}