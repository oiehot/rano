using System.IO;
using UnityEditor;

namespace Rano.Editor.Build
{
    public class AndroidBuilder : Builder
    {
        protected AndroidBuilder(bool developmentBuild) : base(developmentBuild)
        {
            options.target = BuildTarget.Android;
        }

        protected override string GetOutputDirectory()
        {
            return Path.Combine(base.GetOutputDirectory(), "Android");
        }
    }

    public class AndroidBuilderAPK : AndroidBuilder
    {
        public AndroidBuilderAPK(bool developmentBuild, bool autoRun) : base(developmentBuild)
        {
            EditorUserBuildSettings.buildAppBundle = false;
            if (autoRun)
            {
                options.options |= BuildOptions.AutoRunPlayer;
            }
        }

        protected override string GetOutputExtension()
        {
            return ".apk";
        }
    }

    public class AndroidBuilderAAB : AndroidBuilder
    {
        public AndroidBuilderAAB(bool developmentBuild, bool autoRun) : base(developmentBuild)
        {
            EditorUserBuildSettings.buildAppBundle = true;
            if (autoRun)
            {
                options.options |= BuildOptions.AutoRunPlayer;
            }
        }

        protected override string GetOutputExtension()
        {
            return ".aab";
        }        
    }
}