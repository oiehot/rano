using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

namespace Rano
{
    public static class Log
    {
        private enum ELogType
        {
            Default,
            Info,
            Warning,
            Error,
            System,
            Important,
            Todo
        }
        
        private static readonly int FONT_SIZE = 12;
        
        #if UNITY_EDITOR

        private static readonly string DEFAULT_TITLE_COLOR = "#aaaaaaff";
        private static readonly string DEFAULT_CALLER_COLOR = "#aaaaaaff";
        private static readonly string DEFAULT_TEXT_COLOR = "#aaaaaaff";

        private static readonly string INFO_TITLE_COLOR = "#ffffffff";
        private static readonly string INFO_CALLER_COLOR = "#999999ff";
        private static readonly string INFO_TEXT_COLOR = "#999999ff";
        
        private static readonly string WARNING_TITLE_COLOR = "#ffff55ff";
        private static readonly string WARNING_CALLER_COLOR = "#ffff55ff";
        private static readonly string WARNING_TEXT_COLOR = "#ffff55ff";

        private static readonly string ERROR_TITLE_COLOR = "#ff5555ff";
        private static readonly string ERROR_CALLER_COLOR = "#ff5555ff";
        private static readonly string ERROR_TEXT_COLOR = "#ff5555ff";
        
        private static readonly string SYSTEM_TITLE_COLOR = "#ffffffff";
        private static readonly string SYSTEM_CALLER_COLOR = "#00ffffff";
        private static readonly string SYSTEM_TEXT_COLOR = "#00ffffff";

        private static readonly string IMPORTANT_TITLE_COLOR = "#00ff00ff";
        private static readonly string IMPORTANT_CALLER_COLOR = "#00ff00ff";
        private static readonly string IMPORTANT_TEXT_COLOR = "#00ff00ff";

        private static readonly string TODO_TITLE_COLOR = "#ffbb00ff";
        private static readonly string TODO_CALLER_COLOR = "#ffbb00ff";
        private static readonly string TODO_TEXT_COLOR = "#ffbb00ff";
        #endif
        
        private static string ToShortFilepath(string filePath)
        {
            var tokens = filePath.Split(new[] { '/', '\\' });
            var filenameWithExt = tokens[tokens.Length - 1];
            return Path.GetFileNameWithoutExtension(filenameWithExt);
        }
        
        private static string WithColorTag(string text, string color)
        {
            #if UNITY_EDITOR
                return $"<color={color}>{text}</color>";
            #else
                return text;
            #endif
        }

        private static string WithSizeTag(string text, int size)
        {
            #if UNITY_EDITOR
                return $"<size={size}>{text}</size>";
            #else
                return text;
            #endif
        }

        private static string WithThreadSignature(string text)
        {
            return $"[t:{Thread.CurrentThread.ManagedThreadId}] {text}";
        }
        
        private static string GetTitleColor(ELogType logType)
        {
            #if UNITY_EDITOR
                string result = logType switch
                {
                    ELogType.Info => INFO_TITLE_COLOR,
                    ELogType.Warning => WARNING_TITLE_COLOR,
                    ELogType.Error => ERROR_TITLE_COLOR,
                    ELogType.System => SYSTEM_TITLE_COLOR,
                    ELogType.Important => IMPORTANT_TITLE_COLOR,
                    ELogType.Todo => TODO_TITLE_COLOR,
                    _ => DEFAULT_TITLE_COLOR
                };
                return result;
            #else
                return null;
            #endif
        }

        private static string GetCallerColor(ELogType logType)
        {
            #if UNITY_EDITOR
                string result = logType switch
                {
                    ELogType.Info => INFO_CALLER_COLOR,
                    ELogType.Warning => WARNING_CALLER_COLOR,
                    ELogType.Error => ERROR_CALLER_COLOR,
                    ELogType.System => SYSTEM_CALLER_COLOR,
                    ELogType.Important => IMPORTANT_CALLER_COLOR,
                    ELogType.Todo => TODO_CALLER_COLOR,
                    _ => DEFAULT_CALLER_COLOR
                };
                return result;
            #else
                return null;
            #endif
        }

        private static string GetTextColor(ELogType logType)
        {
            #if UNITY_EDITOR
                string result = logType switch
                {
                    ELogType.Info => INFO_TEXT_COLOR,
                    ELogType.Warning => WARNING_TEXT_COLOR,
                    ELogType.Error => ERROR_TEXT_COLOR,
                    ELogType.System => SYSTEM_TEXT_COLOR,
                    ELogType.Important => IMPORTANT_TEXT_COLOR,
                    ELogType.Todo => TODO_TEXT_COLOR,
                    _ => DEFAULT_TEXT_COLOR
                };
                return result;
            #else
                return null;
            #endif
        }

        private static string GetLogTypeText(ELogType logType)
        {
            string result = logType switch
            {
                ELogType.Info => "[INFO]",
                ELogType.Warning => "[WARN]",
                ELogType.Error => "[ERR]",
                ELogType.System => "[SYS]",
                ELogType.Important => "[IMPORTANT]",
                ELogType.Todo => "[TODO]",
                _ => "[UNKNOWN]"
            };
            return result;
        }
        
        private static void Print(ELogType logType, string text, bool caller, string filePath, int line, string member)
        {
            string logTypeText = WithColorTag(GetLogTypeText(logType), GetTitleColor(logType));
            string logText = WithColorTag(text, GetTextColor(logType));
            
            string callerText;
            if (caller) callerText = WithColorTag($"{ToShortFilepath(filePath)}.{member}[{line}]: ", GetCallerColor(logType));
            else callerText = "";
            
            string sizedText = WithSizeTag($"{logTypeText} {callerText}{logText}", FONT_SIZE);
            
            // sizedText = WithThreadSignature(sizedText);

            switch (logType)
            {
                case ELogType.Info:
                    UnityEngine.Debug.Log(sizedText);
                    break;
                case ELogType.Warning:
                    UnityEngine.Debug.LogWarning(sizedText);
                    break;
                case ELogType.Error:
                    UnityEngine.Debug.LogError(sizedText);
                    break;
                default:
                    UnityEngine.Debug.Log(sizedText);
                    break;
            }
        }
        
        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            Print(ELogType.Info, text, caller, filePath, line, member);
        }
        
        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            Print(ELogType.Warning, text, caller, filePath, line, member);
        }
        
        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Error(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {        
            Print(ELogType.Error, text, caller, filePath, line, member);
        }
        
        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Sys(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {  
            Print(ELogType.System, text, caller, filePath, line, member);
        }

        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Important(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {
            Print(ELogType.Important, text, caller, filePath, line, member);
        }

        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Todo(string text, bool caller=true, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
        {  
            Print(ELogType.Todo, text, caller, filePath, line, member);
        }

        [Conditional("ENABLE_LOG"), Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Exception(Exception e)
        {  
            UnityEngine.Debug.LogException(e);
        }
    }
}