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

#if UNITY_EDITOR

        public static int fontSize = 12;
        public static string importantTitleColor = "#ffffffff";
        public static string importantCallerColor = "#999999ff";
        public static string importantTextColor = "#00ff00ff";

        public static string infoTitleColor = "#ffffffff";
        public static string infoCallerColor = "#999999ff";

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

        public static void Important(string text)
        {
            UnityEngine.Debug.Log($"<size={fontSize}><color={importantTitleColor}>[IMPR]</color> <color={importantCallerColor}>{Log.Caller}</color>: <color={importantTextColor}>{text}</color></size>");
        }

        public static void Info(string text)
        {
            UnityEngine.Debug.Log($"<size={fontSize}><color={infoTitleColor}>[INFO]</color> <color={infoCallerColor}>{Log.Caller}</color>: {text}</size>");
        }

        public static void Warning(string text)
        {
            UnityEngine.Debug.LogWarning($"<color=#ffff55ff>{Log.Caller}: {text}</color>");
        }

        public static void Error(string text)
        {
            UnityEngine.Debug.LogError($"<color=#ff5555ff>{Log.Caller}: {text}</color>");
        }

        public static void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

#endif

#if !UNITY_EDITOR

        public static void Important(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[IMPORTANT] {filePath}:{line}.{member}: {text}");
        }

        public static void Info(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[INFO] {filePath}:{line}.{member}: {text}");
        }

        public static void Warning(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[INFO] {filePath}:{line}.{member}: {text}");
        }

        public static void Error(string text, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            UnityEngine.Debug.Log($"[INFO] {filePath}:{line}.{member}: {text}");
        }

        public static void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
#endif

    }
}