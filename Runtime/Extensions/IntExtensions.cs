// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public static class IntExtensions
    {
        public static bool ToBool(this int i)
        {
            return i != 0 ? true : false;
        }
    }
}