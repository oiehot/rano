// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    /// <summary>
    /// 빌드 버젼을 통합적으로 관리해줌
    /// 유니티 시작 직후 해당 클래스의 생성자 실행됨.
    /// ref: https://docs.unity3d.com/kr/530/Manual/RunningEditorCodeOnLaunch.html
    /// </summary>
    [InitializeOnLoad]
    public class VersionManager : IPostprocessBuildWithReport
    {
        private static bool autoIncrease = true;
        private const string autoIncreaseMenuName = "Build/Auto Increase Build Version";
        private static Version lastVersion;
        private static Version currentVersion;

        /// <summary>생성자</summary>
        static VersionManager()
        {
            // 환경세팅에 자동 빌드 버젼 증가여부 저장
            autoIncrease = EditorPrefs.GetBool(autoIncreaseMenuName, true);
            currentVersion = GetCurrentVersion(); 
            lastVersion = currentVersion;
        }

        /// <todo>자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정</todo>
        [MenuItem(autoIncreaseMenuName, true)]
        private static bool SetAutoIncreaseValidate()
        {
            Menu.SetChecked(autoIncreaseMenuName, autoIncrease);
            return true;
        }

        /// <summary>현재 버젼 구조체 얻기</summary>
        public static Version GetCurrentVersion()
        {
            return new Version(PlayerSettings.bundleVersion);
        }

        /// <summary>버젼 구조체로 PlayerSettings를 설정한다</summary>
        public static void ApplyVersion(Version version)
        {
            lastVersion = GetCurrentVersion();
            currentVersion = version;
            PlayerSettings.bundleVersion = version.ToString();
            PlayerSettings.Android.bundleVersionCode = version.buildVersionCode;
            Log.Important($"Change Version: {lastVersion.ToString()} => {currentVersion.ToString()}");            
        }

        /// <summary>버젼 증가시키고 적용하기</summary>
        private static void IncreaseVersion(int majorInc, int minorInc, int buildInc)
        {
            Version version;
            version = GetCurrentVersion();
            version.major += majorInc;
            version.minor += minorInc;
            version.build += buildInc;
            ApplyVersion(version);
        }

        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(autoIncreaseMenuName, false, 1)]
        private static void SetAutoIncrease()
        {
            autoIncrease = !autoIncrease;
            EditorPrefs.SetBool(autoIncreaseMenuName, autoIncrease);
            Log.Info($"Auto Increase Build Version: {autoIncrease}");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        [MenuItem("Build/Check Current Version", false, 2)]
        private static void CheckCurrentVersion()
        {
            Version version = GetCurrentVersion();
            Log.Info($"Build v{version.ToString()}");
        }

        [MenuItem("Build/Edit Version", false, 3)]
        static void OpenEditVersionWindow()
        {
            VersionEditor window = EditorWindow.CreateInstance<VersionEditor>();
            window.SetVersion(VersionManager.GetCurrentVersion());
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowUtility();
        }
    
        /// <summary>
        /// 빌드 후 실행되는 콜백 함수
        /// </summary>
        public int callbackOrder { get { return 0; } }
        public void OnPostprocessBuild(BuildReport report)
        {   
            switch (report.summary.result)
            {
                case BuildResult.Unknown:
                case BuildResult.Succeeded:
                    if (autoIncrease) IncreaseVersion(0, 0, 1);
                    break;
                case BuildResult.Cancelled:
                case BuildResult.Failed:
                default:
                    break;
            }
        }

        /// <summary>
        /// ex) 0.7.3 => 1.0.0
        /// </summary>
        // [MenuItem("Build/Increase Major Version", false, 51)]
        // private static void IncreaseMajorVersion()
        // {
        //     Version version = GetCurrentVersion();
        //     IncreaseVersion(1, -version.minor, -version.build);
        // }

        /// <summary>
        /// ex) 0.7.3 => 0.8.0
        /// </summary>
        // [MenuItem("Build/Increase Minor Version", false, 52)]
        // private static void IncreaseMinorVersion()
        // {
        //     Version version = GetCurrentVersion();
        //     IncreaseVersion(0, 1, -version.build);
        // }

        /// <summary>
        /// ex) 0.7.3 => 0.7.4
        /// </summary>
        // private static void IncreaseBuildVersion()
        // {
        //     IncreaseVersion(0, 0, 1);
        // }

        /// <summary>
        /// 최근에 Increase 하기 전 버젼으로 돌아간다.
        /// </summray>
        // [MenuItem("Build/Undo", false, 53)]
        // public static void RestoreVersion()
        // {
        //     ApplyVersion(lastVersion);
        // }        
    }
}