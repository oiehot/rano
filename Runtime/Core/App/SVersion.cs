using System;
using UnityEngine;

namespace Rano.App
{
    [Serializable]
    public struct SVersion
    {
        private int _major;
        private int _minor;
        private int _build;

        public int Major
        {
            get => _major;
            set => _major = value;
        }

        public int Minor
        {
            get => _minor;
            set => _minor = value;
        }
        
        public int Build
        {
            get => _build;
            set => _build = value;
        }

        public int BuildVersionCode
        {
            get
            {
                if (_minor >= 10)
                {
                    Debug.LogWarning($"Minor 버젼이 10 이상인 경우 Major 자릿수를 침범합니다 ({_minor})");
                }
                return _major * 10000 + _minor * 1000 + _build;
            }
        }
        
        public static SVersion MinimumVersion => new SVersion(0, 0, 0);

        public string FullVersionString => $"{_major}.{_minor}.{_build} ({BuildVersionCode})";

        public SVersion(int major, int minor, int build)
        {
            if (major < 0) major = 0;
            if (minor < 0) minor = 0;
            if (build < 0) build = 0;
            _major = major;
            _minor = minor;
            _build = build;
        }
        
        public SVersion(string versionString)
        {
            string[] tokens;
            tokens = versionString.Split('.');
            if (tokens.Length == 3)
            {
                try
                {
                    _major = int.Parse(tokens[0]);
                    _minor = int.Parse(tokens[1]);
                    _build = int.Parse(tokens[2]);
                }
                catch
                {
                    _major = 0;
                    _minor = 0;
                    _build = 0;
                }
            }
            else if (tokens.Length == 2)
            {
                try
                {
                    _major = int.Parse(tokens[0]);
                    _minor = int.Parse(tokens[1]);
                }
                catch
                {
                    _major = 0;
                    _minor = 0;
                }
                _build = 0;
            }
            else if (tokens.Length == 1)
            {
                try
                {
                    _major = int.Parse(tokens[0]);
                }
                catch
                {
                    _major = 0;
                }
                _minor = 0;
                _build = 0;
            }
            else
            {
                _major = 0;
                _minor = 0;
                _build = 0;
            }
        }

        public override string ToString()
        {
            return $"{_major}.{_minor}.{_build}";
        }

        public override bool Equals(object o)
        {
            SVersion v = (SVersion)o;
            if (BuildVersionCode == v.BuildVersionCode) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return BuildVersionCode;
        }

        public static bool operator == (SVersion a, SVersion b) {
            if (a.BuildVersionCode == b.BuildVersionCode) return true;
            return false;
        }

        public static bool operator != (SVersion a, SVersion b) {
            if (a.BuildVersionCode != b.BuildVersionCode) return true;
            return false;
        }

        public static bool operator >= (SVersion a, SVersion b) {
            if (a.BuildVersionCode >= b.BuildVersionCode) return true;
            return false;
        }

        public static bool operator <= (SVersion a, SVersion b) {
            if (a.BuildVersionCode <= b.BuildVersionCode) return true;
            return false;
        }

        public static bool operator > (SVersion a, SVersion b) {
            if (a.BuildVersionCode > b.BuildVersionCode) return true;
            return false;
        }

        public static bool operator < (SVersion a, SVersion b) {
            if (a.BuildVersionCode < b.BuildVersionCode) return true;
            return false;
        }
    }
}