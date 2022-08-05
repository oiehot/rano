using System;
using System.Globalization;

namespace Rano
{
    public static class NumberExtensions
    {
        public static string ToCommaString(this int value)
        {
            return value.ToString("#,#", CultureInfo.InvariantCulture);
        }

        public static string ToCommaString(this long value)
        {
            return value.ToString("#,#", CultureInfo.InvariantCulture);
        }

        public static string ToUnitString(this int value)
        {
            if (value > 999999999 || value < -999999999)
            {
                // ex) 1.234B
                return value.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999 || value < -999999)
            {
                // ex) 1.23M
                return value.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else if (value > 999 || value < -999)
            {
                // ex) 1.2K
                return value.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string ToUnitString(this long value)
        {
            if (value > 999999999999L || value < -999999999999L)
            {
                // ex) 1.234T
                return value.ToString("0,,,,.###T", CultureInfo.InvariantCulture);
            }
            else if (value > 999999999L || value < -999999999L)
            {
                // ex) 1.234B
                return value.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999L || value < -999999L)
            {
                // ex) 1.23M
                return value.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else if (value > 999L || value < -999L)
            {
                // ex) 1.2K
                return value.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string ToUnitString(this ulong value)
        {
            if (value > 999999999999UL)
            {
                // ex) 1.234T
                return value.ToString("0,,,,.###T", CultureInfo.InvariantCulture);
            }
            else if (value > 999999999UL)
            {
                // ex) 1.234B
                return value.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999UL)
            {
                // ex) 1.23M
                return value.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else if (value > 999UL)
            {
                // ex) 1.2K
                return value.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}