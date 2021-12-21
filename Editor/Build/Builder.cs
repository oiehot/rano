// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Rano;
using Rano.App;

namespace RanoEditor.Build
{
    public class Builder : IBuilder
    {
        protected BuildPlayerOptions _options;

        public Builder()
        {
            _options = new BuildPlayerOptions();
            _options.scenes = BuildHelper.GetEnableScenes();
            _options.locationPathName = null;
            // this.options.target = BuildTarget.iOS;
            _options.options = BuildOptions.None;
                // | BuildOptions.AutoRunPlayer
                // | BuildOptions.Development
                // | BuildOptions.ConnectWithProfiler
                // | BuildOptions.AllowDebugging;
        }

        protected virtual string GetOutputDirectory()
        {   
            DirectoryInfo assetDir = new DirectoryInfo(Application.dataPath);
            DirectoryInfo unityDir = assetDir.Parent;
            return Path.Combine(unityDir.FullName, "Builds");
        }

        protected virtual string GetOutputExtension()
        {
            return "";
        }

        private string GetOutputFilename()
        {
            return $"{Application.productName}_{VersionManager.GetCurrentVersion().ToString()}";
        }

        private string GetOutputPath()
        {
            return $"{GetOutputDirectory()}/{GetOutputFilename()}{GetOutputExtension()}";
        }

        public virtual BuildReport Build()
        {
            BuildReport report;

            // 빌드 아웃풋 디렉토리가 없으면 생성한다.
            DirectoryInfo buildDirectory = new DirectoryInfo(GetOutputDirectory());
            if (!buildDirectory.Exists)
            {
                buildDirectory.Create();
            }

            // 빌드 아웃풋 경로 설정 (파일 경로)
            _options.locationPathName = GetOutputPath();

            // 빌드 시작 전 로그
            string buildVersionStr = VersionManager.GetCurrentVersion().ToString();
            Log.Important($"Building... {buildVersionStr}");
            Log.Info($"Target: {_options.target}");
            Log.Info($"Output: {_options.locationPathName}");

            // 빌드
            report = BuildPipeline.BuildPlayer(_options);
            switch (report.summary.result)
            {
                case BuildResult.Unknown:
                    Log.Important($"Build Completed {buildVersionStr} (with some warnings)");
                    Log.Info($"Output: {_options.locationPathName}");
                    break;

                case BuildResult.Succeeded:
                    Log.Important($"Build Completed {buildVersionStr}");
                    Log.Info($"Output: {_options.locationPathName}");
                    //Log.Info($"{report.summary.totalSize} 바이트");
                    break;

                default:
                    Log.Warning("Build Failed");
                    break;
            }            
            return report;
        }
    }
}