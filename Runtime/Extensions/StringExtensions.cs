// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Text; // StringBuilder

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
    }
}