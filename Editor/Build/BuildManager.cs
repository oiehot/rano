// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.IO;
using UnityEngine;
using UnityEditor;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    public static class BuildManager
    {
        private const string _devBuildMenuName = "Build/Development Build";
        public static bool IsDevelopmentBuild { get; private set; } = true;
        public static string BuildPath { get; private set; }

        static BuildManager()
        {
            string assetPath = UnityEngine.Application.dataPath;
            DirectoryInfo unityDirInfo = System.IO.Directory.GetParent(assetPath);
            string unityProjectPath = unityDirInfo.FullName;
            BuildPath = $"{unityProjectPath}/Builds";
            
            IsDevelopmentBuild = EditorPrefs.GetBool(_devBuildMenuName, true);
        }

        #region 개발빌드 토글

        /// <todo>자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정</todo>
        [MenuItem(_devBuildMenuName, true)]
        private static bool SetDevelopmentBuildValidate()
        {
            Menu.SetChecked(_devBuildMenuName, IsDevelopmentBuild);
            return true;
        }
        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(_devBuildMenuName, false, 100)]
        private static void SetDevelopmentBuild()
        {
            IsDevelopmentBuild = !IsDevelopmentBuild;
            EditorPrefs.SetBool(_devBuildMenuName, IsDevelopmentBuild);
            Log.Info($"Development Build: {IsDevelopmentBuild}");
        }

        #endregion

        [MenuItem("Build/Build Android APK", false, 201)]
        public static void BuildAndroidAPK()
        {
            var builder = new AndroidBuilderAPK(IsDevelopmentBuild);
            builder.Build();
        }

        [MenuItem("Build/Build Android AppBundle", false, 202)]
        public static void BuildAndroidAppBundle()
        {
            var builder = new AndroidBuilderAAB(IsDevelopmentBuild);
            builder.Build();
        }

        [MenuItem("Build/Open Build Folder", false, 300)]
        public static void OpenBuildFolder()
        {
            Log.Info($"Open Build Folder: {BuildPath}");
            EditorUtility.RevealInFinder(BuildPath);
        }
    }
}