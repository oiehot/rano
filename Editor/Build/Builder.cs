#nullable enable

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;

namespace Rano.Editor.Build
{
    public class Builder : IBuilder
    {
        protected BuildPlayerOptions options;

        protected Builder(bool developmentBuild)
        {
            options = new BuildPlayerOptions();
            options.scenes = BuildHelper.GetEnableScenes();
            options.locationPathName = null;
            if (developmentBuild)
            {
                options.options |= BuildOptions.Development;
            }
        }

        protected virtual string? GetOutputDirectory()
        {   
            DirectoryInfo assetDirectory = new DirectoryInfo(Application.dataPath);
            DirectoryInfo? unityDirectory;
            try
            {
                unityDirectory = assetDirectory.Parent;
            }
            catch
            {
                unityDirectory = null;
            }

            if (unityDirectory == null) return null;
            return Path.Combine(unityDirectory.FullName, "Builds");
        }

        protected virtual string GetOutputExtension()
        {
            return "";
        }

        private string GetOutputFilename()
        {
            string filename = $"{Application.productName}_{VersionManager.GetCurrentVersion().ToString()}";
            if (options.options.HasFlag(BuildOptions.Development))
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
            
            string? outputDirectory = GetOutputDirectory();
            if (outputDirectory == null)
            {
                throw new BuildFailedException("아웃풋 디렉토리를 얻을 수 없습니다");
            }
            
            // 빌드 아웃풋 디렉토리가 없으면 생성한다.
            DirectoryInfo buildDirectory = new DirectoryInfo(outputDirectory);
            if (buildDirectory.Exists == false)
            {
                buildDirectory.Create();
            }

            // 빌드 아웃풋 경로 설정 (파일 경로)
            options.locationPathName = GetOutputPath();

            // TODO: GlobalPreferences 또는 AddressableAssetSettings를 통해
            // TODO: 플레이어 빌드 시 어드레서블을 빌드할 수 설정할 수 있다.
            // TODO: 따라서 이 옵션은 필요 없어짐. 추후 제거할것.
            // 어드레서블 빌드
            // if (BuildManager.IsAdressableBuild)
            // {
            //     Log.Important($"Building Addressable Assets...");
            //     PreprocessAddressableBuild();
            //     UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent(
            //         out var addrBuildResult);
            //     if (string.IsNullOrEmpty(addrBuildResult.Error) == false)
            //     {
            //         throw new BuildFailedException($"어드러세블 빌드에 실패했습니다 ({addrBuildResult.Error})");
            //     }
            //
            //     PostprocessAddressableBuild();
            // }

            // 빌드 시작 전 로그
            string buildVersionStr = VersionManager.GetCurrentVersion().ToString();
            Log.Important($"Building... (v{buildVersionStr})");
            Log.Info($"BuildPlayerOptions: {options}");
            Log.Info($"BuildOptions: {options.options}");
            Log.Info($"Target: {options.target}");
            Log.Info($"Output: {options.locationPathName}");

            // 빌드
            report = BuildPipeline.BuildPlayer(options);
            switch (report.summary.result)
            {
                case BuildResult.Unknown:
                    Log.Important($"Build Completed {buildVersionStr} (with some warnings)");
                    Log.Info($"Output: {options.locationPathName}");
                    break;

                case BuildResult.Succeeded:
                    Log.Important($"Build Completed {buildVersionStr}");
                    Log.Info($"Output: {options.locationPathName}");
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