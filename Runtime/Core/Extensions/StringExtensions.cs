using System;
using System.Text;

namespace Rano
{
    public static class StringExtensions
    {
        public static string Repeat(this string source, int multiplier)
        {
            if (multiplier <= 0) {
                return "";
            }
            else {
                StringBuilder sb = new StringBuilder(multiplier * source.Length);
                for (int i = 0; i < multiplier; i++)
                {
                    sb.Append(source);
                }
                return sb.ToString();
            }
        }

        public static DateTime ToDateTime(this string str)
        {
            DateTime result;

            if (str != null)
            {
                try
                {
                    result = DateTime.Parse(str);
                }
                catch
                {
                    result = DateTime.MinValue;
                }
            }
            else
            {
                result = DateTime.MinValue;
            }

            return result;
        }
    }
}