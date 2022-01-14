// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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