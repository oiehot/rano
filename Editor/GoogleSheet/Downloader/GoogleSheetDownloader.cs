#nullable enable

using UnityEditor;
using Rano.Network;

namespace Rano.Editor.GoogleDoc
{
    public static class GoogleSheetDownloader
    {   
        [MenuItem("Build/Download all GoogleSheetItems", false, 500)]
        public static async void DownloadAll()
        {
            var targets =
                AssetDatabaseHelper.GetScriptableObjects<GoogleSheetDownloadItemSO>();
         
            Log.Important($"모든 구글시트 아이템 다운로드 중... (total: {targets.Count})");
            
            foreach (GoogleSheetDownloadItemSO target in targets)
            {
                string? url = target.ExportUrl;
                string? targetAssetPath = target.TargetAssetPath;

                if (string.IsNullOrEmpty(url)) continue;
                if (string.IsNullOrEmpty(targetAssetPath)) continue;

                if (target.includeInBuild == false)
                {
                    Log.Info($"다운로드 생략 (타겟:{targetAssetPath}, IncludeInBuild가 꺼져 있음)");
                    continue;
                }
                
                Log.Info($"다운로드 중 ... (타겟:{targetAssetPath})");
                string? text = await Http.GetStringAsync(url!);
                if (text == null)
                {
                    Log.Warning("다운로드 실패 (네트워크로 부터 다운로드 실패)");
                    continue;
                }
                
                Rano.IO.LocalFile.WriteString(targetAssetPath!, text);
                
                Log.Info($"다운로드 성공");
            }
            Log.Important($"모든 구글시트 아이템 다운로드 완료");
        }
    }
}