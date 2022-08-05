using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Rano.Editor.GoogleDoc
{
    public class GoogleSheetDownloaderOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // GoogleSheetDownloader.DownloadAll();
        }
    }
}