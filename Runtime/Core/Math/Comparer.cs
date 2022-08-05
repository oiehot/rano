using System;
using System.Collections.Generic;

namespace Rano.Math
{
    public class FloatComparer : IEqualityComparer<float>
    {
        private readonly int roundDigits = 2;
        private readonly MidpointRounding roundingMethod = System.MidpointRounding.ToEven;

        public bool Equals(float x, float y)
        {
            if (System.Math.Round(x, roundDigits, roundingMethod) ==
                System.Math.Round(y, roundDigits, roundingMethod))
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
            return System.Math.Round(f, roundDigits, roundingMethod).GetHashCode();
        }
    }
}