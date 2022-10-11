#nullable enable

using System.IO;
using UnityEngine;
using UnityEditor;
using Rano.GoogleSheet;

namespace Rano.Editor.GoogleDoc
{
    [CreateAssetMenu(fileName="GoogleSheetDownloadItem", menuName="Rano/GoogleSheet/GoogleSheet Download Item")]
    public class GoogleSheetDownloadItemSO : ScriptableObject
    {
        [Header("GoogleSheet")]
        public SGoogleSheetId googleSheetId;
        public EGoogleSheetExportFormat exportFormat = EGoogleSheetExportFormat.CSV;
        
        [Header("Download to FileAsset")]
        public TextAsset? targetAsset;
        
        [Header("Download to Directory")]
        public DefaultAsset? targetDirectoryAsset;
        public string? targetFilename;

        [Header("Options")]
        public bool includeInBuild;
        
        public string? ExportUrl => googleSheetId.GetExportUrl(exportFormat);

        public string? TargetAssetPath
        {
            get
            {
                // 에셋링크로 경로 지정.
                if (targetAsset.IsNotNull())
                {
                    string? targetAssetPath = AssetDatabase.GetAssetPath(targetAsset);
                    if (string.IsNullOrEmpty(targetAssetPath))
                    {
                        Log.Warning($"타겟 에셋의 경로를 얻는데 실패");
                        return null;
                    }
                    return targetAssetPath;
                }

                // 디렉토리와 파일이름으로 경로 지정.
                else if (targetDirectoryAsset.IsNotNull())
                {
                    string? targetDirectoryPath = AssetDatabase.GetAssetPath(targetDirectoryAsset);

                    if (string.IsNullOrEmpty(targetDirectoryPath))
                    {
                        Log.Warning($"타겟 디렉토리 경로를 얻는데 실패");
                        return null;
                    }

                    FileAttributes attrs = File.GetAttributes(targetDirectoryPath);
                    bool isDirectory = attrs.HasFlag(FileAttributes.Directory);
                    if (isDirectory == false)
                    {
                        Log.Warning("지정한 타겟 디렉토리가 디렉토리가 아님");
                        return null;
                        
                    }
                    
                    if (string.IsNullOrEmpty(targetFilename))
                    {
                        Log.Warning("저장 할 파일명이 지정되지 않았음");
                        return null;
                    }
                    
                    string ext = exportFormat.ToString().ToLower();
                    return $"{targetDirectoryPath}/{targetFilename}.{ext}";
                }

                else
                {
                    Log.Warning("다운로드 받을 경로에 대한 정보가 없음");
                    return null;
                }
            }
        }
    }
}