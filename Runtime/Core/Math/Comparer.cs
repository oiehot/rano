using System;
using System.Collections.Generic;

namespace Rano.Math
{
    public class FloatComparer : IEqualityComparer<float>
    {
        private const int ROUND_DIGITS = 2;
        private const MidpointRounding ROUNDING_METHOD = MidpointRounding.ToEven;

        public bool Equals(float x, float y)
        {
            if (System.Math.Round(x, ROUND_DIGITS, ROUNDING_METHOD) ==
                System.Math.Round(y, ROUND_DIGITS, ROUNDING_METHOD))
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
            return System.Math.Round(f, ROUND_DIGITS, ROUNDING_METHOD).GetHashCode();
        }
    }
}