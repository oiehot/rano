using UnityEngine;
using UnityEditor;

namespace Rano.Editor
{
    // PSDImporter 6.0.5~6.0.6의 NullException (MaxSize) 에러를 없애기 위한 방법.
    // 1번만 실행해주면 된다.
    // ref: https://forum.unity.com/threads/psb-asset-inspector-window-is-breaking.1334079/
    public static class FixPSDImporter
    {
        [MenuItem("Tools/Reset PSDImporter FoldOut", false)]
        public static void ResetPSDImporterFoldout()
        {
            Debug.Log("ResetPSDImporterFoldout: Start"); 
            UnityEditor.EditorPrefs.DeleteKey("PSDImporterEditor.m_PlatformSettingsFoldout");
            Debug.Log("ResetPSDImporterFoldout: Done");
        }
    }
}