#nullable enable

using System;
using System.IO;
using UnityEditor;

namespace Rano.Editor.Build
{
    public static class BuildManager
    {
        private const int PRIORITY = 200;
        private const string DEVELOPMENT_BUILD_NAME = "Build/Development Build";
        // public const string ADDRESSABLE_BUILD_NAME = "Build/Adressable Build";
        private const string AUTORUN_BUILD_NAME = "Build/Auto Run";
        public static bool IsDevelopmentBuild { get; private set; }
        // public static bool IsAdressableBuild { get; private set; }
        public static bool IsAutoRun { get; private set; }
        public static string BuildPath { get; private set; }

        static BuildManager()
        {
            string assetPath = UnityEngine.Application.dataPath;

            DirectoryInfo? unityDirectory;
            try
            {
                unityDirectory = Directory.GetParent(assetPath);
            }
            catch
            {
                unityDirectory = null;
            }

            if (unityDirectory == null)
            {
                throw new Exception($"상위 디렉토리를 얻을 수 없습니다 ({assetPath})");
            }
            
            string unityProjectPath = unityDirectory.FullName;
            
            BuildPath = $"{unityProjectPath}/Builds";

            // TODO: Remove space and convert slash to dot
            // IsAdressableBuild = EditorPrefs.GetBool(AddressableBuildName, true);
            IsDevelopmentBuild = EditorPrefs.GetBool(DEVELOPMENT_BUILD_NAME, true);
            IsAutoRun = EditorPrefs.GetBool(AUTORUN_BUILD_NAME, false);
        }

        #region ToggleDevelopmentBuild

        /// TODO: 자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정.
        [MenuItem(DEVELOPMENT_BUILD_NAME, true)]
        private static bool SetDevelopmentBuildValidate()
        {
            Menu.SetChecked(DEVELOPMENT_BUILD_NAME, IsDevelopmentBuild);
            return true;
        }
        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(DEVELOPMENT_BUILD_NAME, false, PRIORITY+1)]
        private static void SetDevelopmentBuild()
        {
            IsDevelopmentBuild = !IsDevelopmentBuild;
            EditorPrefs.SetBool(DEVELOPMENT_BUILD_NAME, IsDevelopmentBuild);
            Log.Info($"DevelopmentBuild: {IsDevelopmentBuild}");
        }

        #endregion
        
        // TODO: GlobalPreferences 또는 AddressableAssetSettings를 통해
        // TODO: 플레이어 빌드 시 어드레서블을 빌드할 수 설정할 수 있다.
        // TODO: 따라서 이 옵션은 필요 없어짐. 추후 제거할것.
        // #region ToggleAddressableBuild
        //
        // [MenuItem(AddressableBuildName, true)]
        // private static bool SetAddressableBuildValidate()
        // {
        //     Menu.SetChecked(AddressableBuildName, IsAdressableBuild);
        //     return true;
        // }
        //
        // /// <summary>자동 빌드 버젼 증가 체크</summary>
        // [MenuItem(AddressableBuildName, false, PRIORITY+2)]
        // private static void SetAddressableBuild()
        // {
        //     IsAdressableBuild = !IsAdressableBuild;
        //     EditorPrefs.SetBool(AddressableBuildName, IsAdressableBuild);
        //     Log.Info($"AddressableBuild: {IsAdressableBuild}");
        // }
        //
        // #endregion

        #region AutoRun

        [MenuItem(AUTORUN_BUILD_NAME, true)]
        private static bool SetAutoRunValidate()
        {
            Menu.SetChecked(AUTORUN_BUILD_NAME, IsAutoRun);
            return true;
        }

        [MenuItem(AUTORUN_BUILD_NAME, false, PRIORITY+3)]
        private static void SetAutoRun()
        {
            IsAutoRun = !IsAutoRun;
            EditorPrefs.SetBool(AUTORUN_BUILD_NAME, IsAutoRun);
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