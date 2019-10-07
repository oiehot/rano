// based on https://github.com/pb0/DebugDisabler

#if UNITY_EDITOR
    #define ENABLE_LOG
#elif DEVELOPMENT_BUILD
    #define ENABLE_LOG
#else
    #define DISABLE_LOG
#endif

// 유니티 에디터가 아니라면 아무런 기능을 하지않는 Debug 클래스를 생성하여 로그들을 표시하지 않게 만든다.
#if DISABLE_LOG
using UnityEngine;

public class Debug
{
    public static bool developerConsoleVisible
    {
        get { return UnityEngine.Debug.developerConsoleVisible; }
        set { UnityEngine.Debug.developerConsoleVisible = value; }
    }

    public static bool isDebugBuild
    {
        get { return UnityEngine.Debug.isDebugBuild; }
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void Break()
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void ClearDeveloperConsole()
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DebugBreak()
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void Log(object message)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void Log(object message, Object context)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogError(object message)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogError(object message, Object context)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogException(System.Exception exception)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogException(System.Exception exception, Object context)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogWarning(object message)
    {
    }

    [System.Diagnostics.Conditional("DISABLE_LOG")]
    public static void LogWarning(object message, Object context)
    {
    }
}

#endif