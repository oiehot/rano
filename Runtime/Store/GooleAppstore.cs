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
    public class GoogleAppstore : Appstore
    {
        public GoogleAppstore() : base() {}

        protected override string GetPageUrl(string appId)
        {
            // https://play.google.com/store/apps/details?id=com.oiehot.afo2
            // https://play.google.com/store/apps/details?id=com.oiehot.afo2&hl=en_US&gl=US
            return $"https://play.google.com/store/apps/details?id={appId}";
        }

        protected override string GetBrowserUrl(string appId)
        {
            return $"market://details?id={appId}";
        }

        protected override Version? ParseVersion(string output)
        {
            Version? version;
            
            // ...
            // <span class=“htlgb”>1.0.7</span>
            // ...
            Regex regex = new Regex("<span class=\"htlgb\">([0-9.]+)</span>");
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
