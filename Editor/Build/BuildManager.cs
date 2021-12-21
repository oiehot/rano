// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEditor;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    public class BuildManager
    {
        private const string _developmentBuildMenuName = "Build/Development Build";
        private static bool _developmentBuild = true;

        static BuildManager()
        {
            _developmentBuild = EditorPrefs.GetBool(_developmentBuildMenuName, true);
        }

        #region 개발빌드 토글

        /// <todo>자동 빌드 버젼 증가 속성에 따라 메뉴 활성화 여부 결정</todo>
        [MenuItem(_developmentBuildMenuName, true)]
        private static bool SetDevelopmentBuildValidate()
        {
            Menu.SetChecked(_developmentBuildMenuName, _developmentBuild);
            return true;
        }
        /// <summary>자동 빌드 버젼 증가 체크</summary>
        [MenuItem(_developmentBuildMenuName, false, 2)]
        private static void SetDevelopmentBuild()
        {
            _developmentBuild = !_developmentBuild;
            EditorPrefs.SetBool(_developmentBuildMenuName, _developmentBuild);
            Log.Info($"Development Build: {_developmentBuild}");
        }

        #endregion

        [MenuItem("Build/Build Android APK", false, 12)]
        static void BuildAndroidAPK()
        {
            var builder = new AndroidBuilderAPK(_developmentBuild);
            builder.Build();
        }

        [MenuItem("Build/Build Android AppBundle", false, 13)]
        static void BuildAndroidAppBundle()
        {
            var builder = new AndroidBuilderAAB(_developmentBuild);
            builder.Build();
        }
    }
}