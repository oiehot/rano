using UnityEditor;
using System.IO;

namespace Rano.Editor.Build
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