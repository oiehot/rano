// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Diagnostics;

// <b>Bold</b>
// <i>Italic</i>
// <size=14>Size</size>
// <color=#000000ff>Black</color>

namespace Rano
{
    public static class Log
    {
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
            #if UNITY_EDITOR
            UnityEngine.Debug.Log($"<color=#55ff55ff><b>{Log.Caller}: {text}</b></color>");
            #else
            UnityEngine.Debug.Log($"[IMPORTANT] {Log.Caller}: {text}");
            #endif
        }

        public static void Warning(string text)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.LogWarning($"<color=#ffff55ff>{Log.Caller}: {text}</color>");
            #else
            UnityEngine.Debug.LogWarning($"[WARN] {Log.Caller}: {text}");
            #endif
        }

        public static void Info(string text)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.Log($"<color=#ffffffff>{Log.Caller}: {text}</color>");
            #else
            UnityEngine.Debug.Log($"[INFO] {Log.Caller}: {text}");
            #endif
        }

        public static void Error(string text)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.LogError($"<color=#ff5555ff>{Log.Caller}: {text}</color>");
            #else
            UnityEngine.Debug.LogError($"[ERR] {Log.Caller}: {text}");
            #endif
        }

        public static void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}