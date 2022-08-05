using UnityEngine;
using UnityEditor;
using Rano.App;

namespace Rano.Editor.Build
{
    public class VersionEditor : EditorWindow
    {
        private string version;

        public void SetVersion(string version)
        {
            this.version = version;
        }

        public void SetVersion(SVersion version)
        {
            this.version = version.ToString();
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Change the version and click Update.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            version = EditorGUILayout.TextField("Version: ", version);
            GUILayout.Space(60);
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Update")) {
                    SVersion v = new SVersion(version);
                    VersionManager.ApplyVersion(v);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
            EditorGUILayout.EndHorizontal();
        }
    }
}