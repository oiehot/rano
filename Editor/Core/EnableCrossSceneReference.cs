#if false

using UnityEngine;
using UnityEditor.SceneManagement;

namespace Rano.Editor
{
    public static class FixPSDImporter
    {
        [UnityEditor.InitializeOnLoadMethod]
        public static void ResetPSDImporterFoldout()
        {
            Log.Info("크로스 씬 레퍼런스를 사용합니다.");
            Log.Warning("편집은 되지만 리로드되면 연결고리가 끊깁니다.");
            Log.Warning("guid를 통해 다시 연결해주는 프로세스가 필요합니다.");
            EditorSceneManager.preventCrossSceneReferences = false;
        }
    }
}

#endif