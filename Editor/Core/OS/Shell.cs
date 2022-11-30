using System.Diagnostics;

namespace Rano.Editor
{
	public static class Shell
	{
		public static string ShellAppName
		{
			get
			{
#if UNITY_EDITOR_OSX
				return "bash";
#elif UNITY_EDITOR_WIN
				return "cmd.exe";
#endif
			}
		}

		// static Shell()
		// {
			// if (Application.platform == RuntimePlatform.WindowsEditor)
			// {
			// 	pythonPath = Run("where", "python");
			// }
			// else
			// {
			// 	pythonPath = Run("which", "python");
			// }
		// }
		
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