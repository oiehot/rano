using System;

namespace Rano
{
    public static class FloatExtensions
    {
        public static float Square(this float value)
        {
            return value * value;
        }

        public static float Power(this float value, int exp)
        {
            float ret = value;
            for(int i=1; i<exp; i++){
                ret *= value;
            }
            return ret;
        }

        public static string ToPercentString(this float value)
        {
            return String.Format("{0:0%}", value);
        }
    }
}