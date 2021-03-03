// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.IO;
using Rano;

namespace RanoEditor.Build
{
    public class Builder : IBuilder
    {
        protected BuildPlayerOptions options;

        public Builder()
        {
            this.options = new BuildPlayerOptions();
            this.options.scenes = BuildHelper.GetEnableScenes();
            this.options.locationPathName = null;
            // this.options.target = BuildTarget.iOS;
            this.options.options = BuildOptions.None;
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
            return $"{this.GetOutputDirectory()}/{this.GetOutputFilename()}{this.GetOutputExtension()}";
        }

        public virtual BuildReport Build()
        {
            BuildReport report;

            // 빌드 아웃풋 디렉토리가 없으면 생성한다.
            DirectoryInfo buildDirectory = new DirectoryInfo(this.GetOutputDirectory());
            if (!buildDirectory.Exists)
            {
                buildDirectory.Create();
            }

            // 빌드 아웃풋 경로 설정 (파일 경로)
            this.options.locationPathName = this.GetOutputPath();

            // 빌드 시작전 로그
            Log.Important("Building...");
            Log.Info($"Target: {this.options.target}");
            Log.Info($"Output: {this.options.locationPathName}");
            
            // 빌드
            report = BuildPipeline.BuildPlayer(this.options);
            switch (report.summary.result)
            {
                case BuildResult.Unknown:
                    Log.Important($"Build Completed {PlayerSettings.bundleVersion} (with some warnings)");
                    break;

                case BuildResult.Succeeded:
                    Log.Important($"Build Completed {PlayerSettings.bundleVersion}");
                    // Debug.Log($"{report.summary.totalSize} 바이트");
                    break;

                default:
                    Log.Warning("Build Failed");
                    break;
            }            
            return report;
        }
    }
}