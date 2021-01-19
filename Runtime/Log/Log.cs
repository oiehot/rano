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
                var method = stackTrace.GetFrame(2).GetMethod();
                string methodName = method.Name;
                string className = method.DeclaringType.Name;
                return $"{className}.{methodName}()";
            }
        }

        public static void Important(string txt)
        {
            UnityEngine.Debug.Log($"<color=#55ff55ff><b>{Log.Caller}: {txt}</b></color>");
        }

        public static void Info(string txt)
        {
            UnityEngine.Debug.Log($"<color=#ffffffff>{Log.Caller}: {txt}</color>");
        }

        public static void Warning(string txt)
        {
            UnityEngine.Debug.Log($"<color=#ffff55ff>{Log.Caller}: {txt}</color>");
        }

        public static void Error(string txt)
        {
            UnityEngine.Debug.Log($"<color=#ff5555ff>{Log.Caller}: {txt}</color>");
        }
    }
}
