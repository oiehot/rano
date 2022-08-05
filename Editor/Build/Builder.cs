using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.Build;

namespace Rano.Editor.Build
{
    public class Builder : IBuilder
    {
        protected BuildPlayerOptions _options;

        public Builder(bool developmentBuild)
        {
            _options = new BuildPlayerOptions();
            _options.scenes = BuildHelper.GetEnableScenes();
            _options.locationPathName = null;
            if (developmentBuild)
            {
                _options.options |= BuildOptions.Development;
            }
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
            string filename;
            filename = $"{Application.productName}_{VersionManager.GetCurrentVersion().ToString()}";
            if (_options.options.HasFlag(BuildOptions.Development))
            {
                filename += "_dev";
            }
            return filename;
        }

        private string GetOutputPath()
        {
            return $"{GetOutputDirectory()}/{GetOutputFilename()}{GetOutputExtension()}";
        }

        public virtual BuildReport Build()
        {
            BuildReport report;
            
            // // 모든 BuildScene중 Missing레퍼런스를 찾는다.
            // if (MissingReferenceFinder.FindMissingReferencesInBuildScenes() == false)
            // {
            //     
            // }
            //
            // // 에셋중 Missing레퍼런스를 찾는다.
            // if (MissingReferenceFinder.FindMissingReferencesInAssets() == false)
            // {
            //     
            // }

            // 빌드 아웃풋 디렉토리가 없으면 생성한다.
            DirectoryInfo buildDirectory = new DirectoryInfo(GetOutputDirectory());
            if (!buildDirectory.Exists)
            {
                buildDirectory.Create();
            }

            // 빌드 아웃풋 경로 설정 (파일 경로)
            _options.locationPathName = GetOutputPath();

            // 어드레서블 빌드
            Log.Important($"Building Addressable Assets...");
            PreprocessAddressableBuild();
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent(out var addrBuildResult);
            if (string.IsNullOrEmpty(addrBuildResult.Error) == false)
            {
                throw new BuildFailedException($"어드러세블 빌드에 실패했습니다 ({addrBuildResult.Error})");
            }
            PostprocessAddressableBuild();

            // 빌드 시작 전 로그
            string buildVersionStr = VersionManager.GetCurrentVersion().ToString();
            Log.Important($"Building... {buildVersionStr}");
            Log.Info($"BuildPlayerOptions: {_options}");
            Log.Info($"BuildOptions: {_options.options}");
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

        private void PreprocessAddressableBuild()
        {
            InterfaceHelper.CallAllInterfacesMethod(
                typeof(IPreprocessAddressableBuild),
                nameof(IPreprocessAddressableBuild.OnPreprocessAddressableBuild));
        }

        private void PostprocessAddressableBuild()
        {
            InterfaceHelper.CallAllInterfacesMethod(
                typeof(IPostprocessAddressableBuild),
                nameof(IPostprocessAddressableBuild.OnPostprocessAddressableBuild));
        }
    }
}