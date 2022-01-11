#if false
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

        protected override string GetPageUrl(string bundleId)
        {
            return $"http://itunes.apple.com/lookup?bundleId={bundleId}";
        }

        // TODO: 앱스토어 브라우징시 com.oiehot.bigtree 형식이 아닌
        // TODO: 1350067922 형식의 identifier를 사용해야 한다.
        // TODO: 검사하여 경고하거나 다른 값을 사용할것.
        protected override string GetBrowserUrl(string bundleId)
        {
            throw new System.Exception($"애플 앱스토어 브라우징시 앱스토어 브라우징시 com.oiehot.bigtree 형식이 아닌 1350067922 형식의 ID를 사용해야 한다.");
            // return $"itms-apps://itunes.apple.com/app/id{bundleId}";
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

#endif