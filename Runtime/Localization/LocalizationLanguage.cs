// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;

namespace Rano
{
    public struct LocalizationLanguage
    {
        public string code;
        public SystemLanguage value;

        public LocalizationLanguage(SystemLanguage lang)
        {
            value = lang;

            switch (lang)
            {
                case SystemLanguage.English:
                    code = "EN";
                    break;
                case SystemLanguage.Korean:
                    code = "KR";
                    break;
                default:
                    code = "?";
                    break;
            }
        }

        public override string ToString()
        {
            return $"{value.ToString()}({code})";
        }

        public override bool Equals(object obj) 
        {
            if (!(obj is LocalizationLanguage))
                return false;

            LocalizationLanguage b = (LocalizationLanguage)obj;
            return value == b.value;
        }

        public override int GetHashCode()
        {
            return (int)value;
        }

        public static bool operator ==(LocalizationLanguage a, LocalizationLanguage b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(LocalizationLanguage a, LocalizationLanguage b)
        {
            return a.value != b.value;
        }        
    }
}