using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Rano.App;

namespace Rano.Editor.Build
{
    /// <summary>
    /// 빌드 버젼을 통합적으로 관리해줌
    /// 유니티 시작 직후 해당 클래스의 생성자 실행됨.
    /// ref: https://docs.unity3d.com/kr/530/Manual/RunningEditorCodeOnLaunch.html
    /// </summary>
    [InitializeOnLoad]
    public class VersionManager : IPostprocessBuildWithReport
    {
        private const int PRIORITY = 100;
        private const string AUTO_INCREASE_MENU_NAME = "Build/Auto Increase Build Version";
        public static bool IsAutoIncrease { get; private set; } = true;
        public static SVersion LastVersion { get; private set; }
        public static SVersion CurrentVersion { get; private set; }
        public static string CurrentVersionString
        {
            get
            {
                SVersion version = GetCurrentVersion();
                if (BuildManager.IsDevelopmentBuild)
                    return $"{version.ToString()}_dev";
                else
                    return $"{version.ToString()}";
            }
        }

        static VersionManager()
        {
            // 환경세팅에 자동 빌드 버젼 증가여부 저장
            IsAutoIncrease = EditorPrefs.GetBool(AUTO_INCREASE_MENU_NAME, true);
            CurrentVersion = GetCurrentVersion(); 
            LastVersion = CurrentVersion;
        }

        /// TODO: 자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정
        [MenuItem(AUTO_INCREASE_MENU_NAME, true, PRIORITY+1)]
        private static bool SetAutoIncreaseValidate()
        {
            Menu.SetChecked(AUTO_INCREASE_MENU_NAME, IsAutoIncrease);
            return true;
        }

        public static SVersion GetCurrentVersion()
        {
            return new SVersion(PlayerSettings.bundleVersion);
        }

        public static void ApplyVersion(SVersion version)
        {
            LastVersion = GetCurrentVersion();
            CurrentVersion = version;
            PlayerSettings.bundleVersion = version.ToString();
            PlayerSettings.Android.bundleVersionCode = version.buildVersionCode;
            PlayerSettings.iOS.buildNumber = version.buildVersionCode.ToString();
            Log.Info($"Change Version: {LastVersion.ToString()} => {CurrentVersion.ToString()}");            
        }

        private static void IncreaseVersion(int majorInc, int minorInc, int buildInc)
        {
            SVersion version;
            version = GetCurrentVersion();
            version.major += majorInc;
            version.minor += minorInc;
            version.build += buildInc;
            ApplyVersion(version);
        }

        /// <summary>
        /// 자동 빌드 버젼 증가 체크
        /// </summary>
        [MenuItem(AUTO_INCREASE_MENU_NAME, false, PRIORITY+1)]
        private static void SetAutoIncrease()
        {
            IsAutoIncrease = !IsAutoIncrease;
            EditorPrefs.SetBool(AUTO_INCREASE_MENU_NAME, IsAutoIncrease);
            Log.Info($"Auto Increase Build Version: {IsAutoIncrease}");
        }

        [MenuItem("Build/Check Current Version", false, PRIORITY+2)]
        private static void CheckCurrentVersion()
        {
            Log.Info($"Build v{CurrentVersionString}");
        }

        [MenuItem("Build/Edit Version", false, PRIORITY+3)]
        static void OpenEditVersionWindow()
        {
            VersionEditor window = EditorWindow.CreateInstance<VersionEditor>();
            window.SetVersion(VersionManager.GetCurrentVersion());
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowUtility();
        }
    
        public int callbackOrder { get { return 0; } }
        public void OnPostprocessBuild(BuildReport report)
        {   
            switch (report.summary.result)
            {
                case BuildResult.Unknown:
                case BuildResult.Succeeded:
                    if (IsAutoIncrease) IncreaseVersion(0, 0, 1);
                    break;
                case BuildResult.Cancelled:
                case BuildResult.Failed:
                default:
                    break;
            }
        }
    }
}