#if false
namespace Rano.Asset
{
    using UnityEngine;
    using UnityEditor;
    class ExcelToJson : AssetPostprocessor 
    {    
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
        {
            foreach (string path in importedAssets)
            {   
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                    if ( filename.StartsWith("~") )
                    {
                        Debug.Log($"Ignore: {filename}");
                        continue; // ~로 시작하면 작업중인 파일이므로 무시한다.
                    }
                    
                    string ext = System.IO.Path.GetExtension(path);           
                    if ( !ext.ToLower().StartsWith(".xls") ) continue; // 엑셀파일만
                    
                    string projectPath = System.IO.Directory.GetParent(Application.dataPath).FullName;
                    
                    string dir = System.IO.Path.GetDirectoryName(path);
                    const string script = "o:/project/afo2/bin/xls2json.py";
                    string xlsPath = $"{projectPath}/{path}";
                    string jsonPath = $"{projectPath}/{dir}/{filename}.json";
                    
                    // TODO: Custom AssetImport(xls), make property like (bool convertToJson, string worksheetName=StageData)
                    
                    string result;
                    string worksheet = "StageData";
                    int exitCode;
                    
                    exitCode = Rano.OS.Shell.Python(script, $"--xlsPath {xlsPath} --jsonPath {jsonPath} --sheetName {worksheet}", out result);
                    Debug.Log($"Result: {result}");
                    Debug.Log($"Exit code: {exitCode}");
            }
        }
    }
}
#endif