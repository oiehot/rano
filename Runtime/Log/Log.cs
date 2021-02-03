using System.Diagnostics;

// <b>Bold</b>
// <i>Italic</i>
// <size=14>Size</size>
// <color=#000000ff>Black</color>

namespace Rano
{   
    public static class SysLog
    {
        public static string Caller
        {
            get
            {
                StackTrace stackTrace = new StackTrace();
                var method = stackTrace.GetFrame(2).GetMethod();
                string methodName = method.Name;
                string className = method.DeclaringType.Name;
                return $"{className}.{methodName}()";
            }
        }

        public static void Important(string text)
        {
            UnityEngine.Debug.Log($"<color=#55ff55ff><b>{SysLog.Caller}: {text}</b></color>");
        }

        public static void Info(string text)
        {
            UnityEngine.Debug.Log($"<color=#ffffffff>{SysLog.Caller}: {text}</color>");
        }

        public static void Warning(string text)
        {
            UnityEngine.Debug.Log($"<color=#ffff55ff>{SysLog.Caller}: {text}</color>");
        }

        public static void Error(string text)
        {
            UnityEngine.Debug.Log($"<color=#ff5555ff>{SysLog.Caller}: {text}</color>");
        }
    }
}
