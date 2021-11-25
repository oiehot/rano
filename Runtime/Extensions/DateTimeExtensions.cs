// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 사람이 읽기 쉬운 시간문자열을 리턴한다.
        /// </summary>
        /// <param name="timeSpan">시간</param>
        /// <returns>사람이 읽기 쉬운 시간문자열</returns>
        /// <example>
        /// TimeSpan timeSpan = new TimeSpan(hours:4, minutes:33, seconds:10);
        /// timeSpan.ToHumanReadableString());
        /// >>> 4 hours 33 minutes 10 seconds
        /// </example>
        public static string ToHumanReadableString(this TimeSpan timeSpan)
        {
            string result = "";
            int d = timeSpan.Days;
            int h = timeSpan.Hours;
            int m = timeSpan.Minutes;
            int s = timeSpan.Seconds;

            if (d > 0)
            {
                if (d == 1) result += $"{d} day";
                else result += $"{d} days";
            }
            if (h > 0)
            {
                if (d > 0) result += " ";
                if (h == 1) result += $"{h} hour";
                else result += $"{h} hours";
            }
            if (m > 0)
            {
                if (h > 0) result += " ";
                if (m == 1) result += $"{m} minute";
                else result += $"{m} minutes";
            }
            if (s > 0)
            {
                if (m > 0) result += " ";
                if (s == 1) result += $"{s} second";
                else result += $"{s} seconds";
            }

            return result;
        }

        /// <summary>
        /// 읽기 쉬운 시간문자열을 리턴한다.
        /// </summary>
        /// <param name="timeSpan">시간</param>
        /// <returns>읽기 쉬운 시간문자열</returns>
        /// <example>
        /// TimeSpan timeSpan = new TimeSpan(hours:4, minutes:33, seconds:10);
        /// timeSpan.ToReadbleString());
        /// >>> 04:33:10
        /// </example>
        public static string ToReadableString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}