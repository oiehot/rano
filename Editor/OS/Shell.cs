// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Rano;

namespace RanoEditor.OS
{
	public static class Shell
	{
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

		//public static int Python(string scriptPath, string arg, out string result)
		//{
		//	UnityEngine.Debug.Log($"python {scriptPath} {arg}");
		//	return Run("python", $"{scriptPath} {arg}", out result);
		//}
	}
}