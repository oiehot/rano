using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace Rano.System
{
	public static class Shell
	{
		// static string pythonPath;
		
		static Shell()
		{
			// if (Application.platform == RuntimePlatform.WindowsEditor)
			// {
			// 	pythonPath = Run("where", "python");
			// }
			// else
			// {
			// 	pythonPath = Run("which", "python");
			// }
		}
		
		public static int Run(string filepath, string arg, out string result)
		{
			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = filepath;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.Arguments = arg;
			
			Process proc = Process.Start(info);
			result = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();
			
			return proc.ExitCode;
		}
		
		public static int Python(string scriptPath, string arg, out string result)
		{
			UnityEngine.Debug.Log($"python {scriptPath} {arg}");
			return Run("python", $"{scriptPath} {arg}", out result);
		}
		
		// [MenuItem("Rano/Test/Python Test")]
		// public static void PythonTest()
		// {
		// 	string result;
		// 	int exitCode = Python("o:/project/afo2/bin/xls2json.py","--xlsPath o:/project/afo2/unity/Assets/Data/StageData01.xlsx --jsonPath o:/project/afo2/unity/Assets/Data/StageData01.json --sheetName StageData", out result);
		// 	// UnityEngine.Debug.Log(exitCode);
		// 	// UnityEngine.Debug.Log(result);
		// }
	}
}