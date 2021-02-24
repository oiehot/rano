using UnityEditor;

namespace Rano.Editor.Build
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