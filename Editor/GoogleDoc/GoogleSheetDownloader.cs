#if false
using System.IO;
using UnityEditor;
using Rano.Network;

namespace Rano.Editor.GoogleDoc
{
    public static class GoogleSheetDownloader
    {   
        [MenuItem("Build/Download All GoogleSheetItems", false, 500)]
        public static void DownloadAll()
        {
            var targets =
                AssetDatabaseHelper.GetScriptableObjects<GoogleSheetDownloadItemSO>();
         
            Log.Sys($"Download All GoogleSheetItems Start (total: {targets.Count})", caller:false);
            foreach (var target in targets)
            {
                string url = target.ExportUrl;
                string targetAssetPath = target.TargetAssetPath;

                if (!target.includeInBuild)
                {
                    Log.Info($"Skip <b>{target.id}</b> (not include in build)", caller:false);
                    continue;
                }

                if (File.Exists(targetAssetPath) && !target.overwrite)
                {
                    Log.Info($"Skip <b>{target.id}</b> (already, don't overwrite)", caller:false);
                    continue;
                }
                
                Log.Info($"Downloading <b>{target.id}</b> (<i>{targetAssetPath}</i>)", caller:false);
                var text = Http.GetString(target.ExportUrl);
                // var task = Http.GetStringAsync(target.ExportUrl);
                // await task;
                // string text = task.Result;
                Rano.IO.LocalFile.WriteString(targetAssetPath, text);
            }
            Log.Sys($"Download All GoogleSheetItems End", caller:false);
        }
    }
}
#endif