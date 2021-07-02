// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano
{
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {
        public class LoadingManagerException : Exception
        {
            public LoadingManagerException()
            {
            }

            public LoadingManagerException(string message) : base(message)
            {
            }

            public LoadingManagerException(string message, Exception inner) : base(message, inner)
            {
            }
        }
    }
}