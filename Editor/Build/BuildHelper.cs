using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Rano.Editor.Build
{
    public static class BuildHelper
    {
        public static string[] GetEnableScenes()
        {
            List<string> EditorScenes = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                EditorScenes.Add(scene.path);
            }

            return EditorScenes.ToArray();
        }
    }
}