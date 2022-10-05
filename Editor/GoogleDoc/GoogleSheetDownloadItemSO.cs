#if false
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Rano.GoogleDoc;

namespace Rano.Editor.GoogleDoc
{
    [CreateAssetMenu(fileName="GoogleSheetDownloadItem", menuName="Rano/GoogleDoc/GoogleSheet Download Item")]
    public class GoogleSheetDownloadItemSO : ScriptableObject
    {
        [Header("Ids")]
        public string id;
        public GoogleSheetId googleSheetId;
        public GoogleSheetExportFormat exportFormat;
        
        [Header("Download to FileAsset")]
        public TextAsset targetAsset;
        
        [Header("Download to Directory")]
        public DefaultAsset targetDirectoryAsset;
        // public string targetFilename;

        [Header("Options")]
        public bool overwrite = true;
        public bool includeInBuild;
        
        public string ExportUrl
        {
            get
            {
                string url = GoogleSheet.GetExportUrl(googleSheetId, exportFormat);
                return url;
            }
        }

        public string TargetAssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("id가 지정되어있지 않습니다.");
                }

                // 에셋링크로 경로 지정.
                if (targetAsset.IsNotNull())
                {
                    string targetAssetPath = AssetDatabase.GetAssetPath(targetAsset);
                    if (string.IsNullOrEmpty(targetAssetPath))
                        throw new Exception($"타겟 에셋의 경로를 얻는데 실패함. ({targetAsset})");
                    return targetAssetPath;
                }

                // 디렉토리와 파일이름으로 경로 지정.
                else if (targetDirectoryAsset.IsNotNull())
                {
                    string dirPath = AssetDatabase.GetAssetPath(targetDirectoryAsset);
                    var attrs = File.GetAttributes(dirPath);
                    bool isDirectory = attrs.HasFlag(FileAttributes.Directory);
                    if (isDirectory)
                    {
                        string ext = exportFormat.ToString().ToLower();
                        return $"{dirPath}/{id}.{ext}";
                    }
                    else
                    {
                        throw new Exception("다운로드 받을 디렉토리가 파일타입일 수는 없음.");
                    }
                }

                else
                {
                    throw new Exception("다운로드 받을 경로에 대한 정보가 없음.");
                }
            }
        }
    }
}
#endif