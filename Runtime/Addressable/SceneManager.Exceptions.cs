// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using Rano;

namespace Rano.Addressable
{
    public partial class SceneManager : MonoSingleton<SceneManager>
    {
        public class SceneOperationException : Exception
        {
            public SceneOperationException()
            {
            }

            public SceneOperationException(string message) : base(message)
            {
            }

            public SceneOperationException(string message, Exception inner) : base(message, inner)
            {
            }
        }
    }
}