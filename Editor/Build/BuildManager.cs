// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.IO;
using UnityEngine;
using UnityEditor;
using Rano;
using RanoEditor.OS;

namespace RanoEditor.Build
{
    public static class BuildManager
    {
        private const int PRIORITY = 200;
        public const string DevelopmentBuildName = "Build/Development Build";
        public const string AddressableBuildName = "Build/Adressable Build";
        public const string AutoRunBuildName = "Build/Auto Run";
        public static bool IsDevelopmentBuild { get; private set; }
        public static bool IsAdressableBuild { get; private set; }
        public static bool IsAutoRun { get; private set; }
        public static string BuildPath { get; private set; }

        static BuildManager()
        {
            string assetPath = UnityEngine.Application.dataPath;
            DirectoryInfo unityDirInfo = System.IO.Directory.GetParent(assetPath);
            string unityProjectPath = unityDirInfo.FullName;
            BuildPath = $"{unityProjectPath}/Builds";

            // TODO: Remove space and convert slash to dot
            IsDevelopmentBuild = EditorPrefs.GetBool(DevelopmentBuildName, true);
            IsAdressableBuild = EditorPrefs.GetBool(AddressableBuildName, true);
            IsAutoRun = EditorPrefs.GetBool(AutoRunBuildName, false);
        }

        #region ToggleDevelopmentBuild

        /// <todo>자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정</todo>
        [MenuItem(DevelopmentBuildName, true)]
        private static bool SetDevelopmentBuildValidate()
        {
            Menu.SetChecked(DevelopmentBuildName, IsDevelopmentBuild);
            return true;
        }
        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(DevelopmentBuildName, false, PRIORITY+1)]
        private static void SetDevelopmentBuild()
        {
            IsDevelopmentBuild = !IsDevelopmentBuild;
            EditorPrefs.SetBool(DevelopmentBuildName, IsDevelopmentBuild);
            Log.Info($"DevelopmentBuild: {IsDevelopmentBuild}");
        }

        #endregion

        #region ToggleAddressableBuild

        [MenuItem(AddressableBuildName, true)]
        private static bool SetAddressableBuildValidate()
        {
            Menu.SetChecked(AddressableBuildName, IsAdressableBuild);
            return true;
        }

        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(AddressableBuildName, false, PRIORITY+2)]
        private static void SetAddressableBuild()
        {
            IsAdressableBuild = !IsAdressableBuild;
            EditorPrefs.SetBool(AddressableBuildName, IsAdressableBuild);
            Log.Info($"AddressableBuild: {IsAdressableBuild}");
        }

        #endregion

        #region AutoRun

        [MenuItem(AutoRunBuildName, true)]
        private static bool SetAutoRunValidate()
        {
            Menu.SetChecked(AutoRunBuildName, IsAutoRun);
            return true;
        }

        [MenuItem(AutoRunBuildName, false, PRIORITY+3)]
        private static void SetAutoRun()
        {
            IsAutoRun = !IsAutoRun;
            EditorPrefs.SetBool(AutoRunBuildName, IsAutoRun);
            Log.Info($"AutoRun: {IsAutoRun}");
        }

        #endregion

        [MenuItem("Build/Build Android APK", false, PRIORITY+21)]
        public static void BuildAndroidAPK()
        {
            var builder = new AndroidBuilderAPK(IsDevelopmentBuild, IsAutoRun);
            builder.Build();
        }

        [MenuItem("Build/Build Android AppBundle", false, PRIORITY+22)]
        public static void BuildAndroidAppBundle()
        {
            var builder = new AndroidBuilderAAB(IsDevelopmentBuild, IsAutoRun);
            builder.Build();
        }

        [MenuItem("Build/Open Build Folder", false, PRIORITY+33)]
        public static void OpenBuildFolder()
        {
            Log.Info($"Open Build Folder: {BuildPath}");
            EditorUtility.RevealInFinder(BuildPath);
        }
    }
}