// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEditor;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    public class BuildManager
    {
        [MenuItem("Build/Build Android APK", false, 11)]
        static void BuildAndroidAPK()
        {
            var builder = new AndroidBuilderAPK();
            builder.Build();
        }

        [MenuItem("Build/Build Android AppBundle", false, 12)]
        static void BuildAndroidAppBundle()
        {
            var builder = new AndroidBuilderAAB();
            builder.Build();
        }
    }
}