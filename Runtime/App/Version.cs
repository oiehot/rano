// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;
using Rano;

namespace Rano.App
{
    [System.Serializable]
    public struct Version
    {
        public int major;
        public int minor;
        public int build;

        public int buildVersionCode
        {
            get
            {
                if (minor >= 10)
                {
                    Debug.LogWarning($"Minor 버젼이 10 이상인 경우 Major 자릿수를 침범합니다 ({minor})");
                }
                return major * 10000 + minor * 1000 + build;
            }
        }

        public string fullVersion
        {
            get
            {
                return $"{major}.{minor}.{build} ({buildVersionCode})";
            }
        }

        public Version(int majorVer, int minorVer, int buildVer)
        {
            if (majorVer < 0) majorVer = 0;
            if (minorVer < 0) minorVer = 0;
            if (buildVer < 0) buildVer = 0;
            major = majorVer;
            minor = minorVer;
            build = buildVer;
        }

        public Version(string versionString)
        {
            string[] tokens;
            tokens = versionString.Split('.');
            if (tokens.Length == 3)
            {
                try
                {
                    major = int.Parse(tokens[0]);
                    minor = int.Parse(tokens[1]);
                    build = int.Parse(tokens[2]);
                }
                catch
                {
                    major = 0;
                    minor = 0;
                    build = 0;
                }
            }
            else if (tokens.Length == 2)
            {
                try
                {
                    major = int.Parse(tokens[0]);
                    minor = int.Parse(tokens[1]);
                }
                catch
                {
                    major = 0;
                    minor = 0;
                }
                build = 0;
            }
            else if (tokens.Length == 1)
            {
                try
                {
                    major = int.Parse(tokens[0]);
                }
                catch
                {
                    major = 0;
                }
                minor = 0;
                build = 0;
            }
            else
            {
                major = 0;
                minor = 0;
                build = 0;
            }
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{build}";
        }

        public override bool Equals(object o)
        {
            Version v = (Version)o;
            if (buildVersionCode == v.buildVersionCode) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return buildVersionCode;
        }

        public static bool operator == (Version a, Version b) {
            if (a.buildVersionCode == b.buildVersionCode) return true;
            else return false;
        }

        public static bool operator != (Version a, Version b) {
            if (a.buildVersionCode != b.buildVersionCode) return true;
            else return false;
        }

        public static bool operator >= (Version a, Version b) {
            if (a.buildVersionCode >= b.buildVersionCode) return true;
            else return false;
        }

        public static bool operator <= (Version a, Version b) {
            if (a.buildVersionCode <= b.buildVersionCode) return true;
            else return false;
        }

        public static bool operator > (Version a, Version b) {
            if (a.buildVersionCode > b.buildVersionCode) return true;
            else return false;
        }

        public static bool operator < (Version a, Version b) {
            if (a.buildVersionCode < b.buildVersionCode) return true;
            else return false;
        }
    }
}