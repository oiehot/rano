// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public static class BoolExtensions
    {
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }
    }
}