// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// <b>Bold</b>
// <i>Italic</i>
// <size=14>Size</size>
// <color=#000000ff>Black</color>

namespace Rano
{
    public static class Log
    {
        private static string ToShortFilepath(string filePath)
        {
            string[] tokens = filePath.Split('/');
            string filenameWithExt = tokens[tokens.Length - 1];
            return System.IO.Path.GetFileNameWithoutExtension(filenameWithExt);
        }

#if UNITY_EDITOR

        public static int fontSize = 12;

        public static string systemTitleColor = "#ffffffff";
        public static string systemCallerColor = "#00ffffff";
        public static string systemTextColor = "#00ffffff";

        public static string importantTitleColor = "#ffffffff";
        public static string importantCallerColor = "#00ff00ff";
        public static string importantTextColor = "#00ff00ff";

        public static string infoTitleColor = "#ffffffff";
        public static string infoCallerColor = "#999999ff";
        public static string infoTextColor = "#999999ff";

        public static string warningTitleColor = "#ffff55ff";
        public static string warningCallerColor = "#ffff55ff";
        public static string warningTextColor = "#ffff55ff";

        public static string errorTitleColor = "#ff5555ff";
        public static string errorCallerColor = "#ff5555ff";
        public static string errorTextColor = "#ff5555ff";

        public static string Caller
        {
            get
            {
                StackTrace stackTrace = new StackTrace();
                System.Reflection.MethodBase method;

                try
                {
                    method = stackTrace.GetFrame(2).GetMethod();
                }
                catch
                {
                    return $"UnknownClass.UnknownMethod()";
                }
                string methodName = method.Name;
                string className = method.DeclaringType.Name;
                return $"{className}.{methodName}()";
            }
        }

        public static void Sys(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            if (caller == true)
            {
                UnityEngine.Debug.Log($"<size={fontSize}><color={systemTitleColor}>[SYS]</color> <color={systemCallerColor}>{ToShortFilepath(filePath)}.{member}[{line}]</color>: <color={systemTextColor}>{text}</color></size>");
            }
            else
            {
                UnityEngine.Debug.Log($"<size={fontSize}><color={systemTitleColor}>[SYS]</color> <color={systemTextColor}>{text}</color></size>");
            }
        }

        public static void Important(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            //UnityEngine.Debug.Log($"<size={fontSize}><color={importantTitleColor}>[IMPR]</color> <color={importantCallerColor}>{Log.Caller}</color>: <color={importantTextColor}>{text}</color></size>");
            UnityEngine.Debug.Log($"<size={fontSize}><color={importantTitleColor}>[IMPR]</color> <color={importantCallerColor}>{ToShortFilepath(filePath)}.{member}[{line}]</color>: <color={importantTextColor}>{text}</color></size>");
        }

        public static void Info(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"<size={fontSize}><color={infoTitleColor}>[INFO]</color> <color={infoCallerColor}>{ToShortFilepath(filePath)}.{member}[{line}]</color>: <color={infoTextColor}>{text}</color></size>");
        }

        public static void Warning(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"<size={fontSize}><color={warningTitleColor}>[WARN]</color> <color={warningCallerColor}>{ToShortFilepath(filePath)}.{member}[{line}]</color>: <color={warningTextColor}>{text}</color></size>");
        }

        public static void Error(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"<size={fontSize}><color={errorTitleColor}>[ERR]</color> <color={errorCallerColor}>{ToShortFilepath(filePath)}.{member}[{line}]</color>: <color={errorTextColor}>{text}</color></size>");
        }

        public static void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

#else

        public static void Sys(string text, bool caller=false, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            if (caller == true)
            {
                UnityEngine.Debug.Log($"[SYS] {ToShortFilepath(filePath)}.{member}[{line}]: {text}");
            }
            else
            {
                UnityEngine.Debug.Log($"[SYS] {text}");
            }
        }

        public static void Important(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            
            UnityEngine.Debug.Log($"[IMPORTANT] {ToShortFilepath(filePath)}.{member}[{line}]:  {text}");
        }

        public static void Info(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[INFO] {ToShortFilepath(filePath)}.{member}[{line}]:  {text}");
        }

        public static void Warning(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[WARN] {ToShortFilepath(filePath)}.{member}[{line}]:  {text}");
        }

        public static void Error(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[ERR] {ToShortFilepath(filePath)}.{member}[{line}]:  {text}");
        }

        public static void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
#endif

    }
}