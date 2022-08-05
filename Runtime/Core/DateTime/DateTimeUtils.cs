using System;

namespace Rano
{
    public static class DateTimeUtils
    {
        public static string CurrentDateTimeString()
        {
            return System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"); // HH:24hour, hh:12hour
        }

        public static DateTime CurrentDateTime()
        {
            return System.DateTime.Now;
        }
    }
}