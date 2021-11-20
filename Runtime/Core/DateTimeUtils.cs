// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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