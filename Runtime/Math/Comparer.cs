// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;

namespace Rano
{
    public class FloatComparer : IEqualityComparer<float>
    {
        private readonly int roundDigits = 2;
        private readonly MidpointRounding roundingMethod = System.MidpointRounding.ToEven;

        public bool Equals(float x, float y)
        {
            if (Math.Round(x, roundDigits, roundingMethod) ==
                Math.Round(y, roundDigits, roundingMethod))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(float f)
        {
            return Math.Round(f, roundDigits, roundingMethod).GetHashCode();
        }
    }
}