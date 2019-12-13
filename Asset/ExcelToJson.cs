// 에셋 포스트 프로세서
// .xls 파일이 감지되면 json으로 변환 (외부 쉘 명령으로)
// ~로 시작하는 .xls 파일은 작업중이므로 무시
// 날아감 ㅜㅜ

using UnityEngine;
using UnityEditor;

class ExcelToJson : AssetPostprocessor 
{
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
		foreach (string path in importedAssets)
		{   
            string ext = System.IO.Path.GetExtension(path);
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);
            
            if ( !ext.ToLower().StartsWith(".xls") ) continue; // 엑셀파일만
            if ( filename.StartsWith("~") ) continue; // ~로 시작하면 작업중인 파일이므로 무시한다.
            
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