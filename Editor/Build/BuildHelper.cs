using UnityEditor;
using System.Collections.Generic;

namespace Rano.Editor.Build
{
    public static class BuildHelper
    {
        public static string[] GetEnableScenes()
        {
            List<string> result = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                result.Add(scene.path);
            }

            return result.ToArray();
        }
    }
}