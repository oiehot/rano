// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.IO;
using UnityEditor;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    public class AndroidBuilder : Builder
    {
        public AndroidBuilder()
        {
            this.options.target = BuildTarget.Android;        
        }

        protected override string GetOutputDirectory()
        {
            return Path.Combine(base.GetOutputDirectory(), "Android");
        }
    }

    public class AndroidBuilderAPK : AndroidBuilder
    {
        public AndroidBuilderAPK()
        {
            EditorUserBuildSettings.buildAppBundle = false;
        }

        protected override string GetOutputExtension()
        {
            return ".apk";
        }
    }

    public class AndroidBuilderAAB : AndroidBuilder
    {
        public AndroidBuilderAAB()
        {
            EditorUserBuildSettings.buildAppBundle = true;
        }

        protected override string GetOutputExtension()
        {
            return ".aab";
        }        
    }
}