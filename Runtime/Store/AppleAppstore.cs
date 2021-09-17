// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Rano.App;

namespace Rano.Store
{
    public class AppleAppstore : Appstore
    {
        public AppleAppstore() : base() {}

        protected override string GetPageUrl(string appId)
        {
            return $"http://itunes.apple.com/lookup?id={appId}";
        }

        protected override string GetBrowserUrl(string appId)
        {
            return $"itms-apps://itunes.apple.com/app/id{appId}";
        }

        protected override Version? ParseVersion(string output)
        {
            Version? version;
            
            // ...
            // "version":1.0.7
            // ...
            Regex regex = new Regex("\"version\":\"([0-9.]+)\"");
            Match m = regex.Match(output);
            if (m.Success)
            {
                version = new Version(m.Groups[1].Value);
            }
            else
            {
                version = null;
            }
            return version;
        }
    }
}
