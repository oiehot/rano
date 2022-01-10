// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano.Network
{
    public class HttpRequestException : Exception
    {
        public HttpRequestException()
        {
        }

        public HttpRequestException(string message)
            : base(message)
        {
        }

        public HttpRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}