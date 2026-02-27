using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class Debugger
{
    public static void Log(object message, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.Log(message + " ( " + classN + ".cs - " + caller.Name + "() )", context);
#endif
    }

    public static void LogWarning(object warningMessage, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.LogWarning(warningMessage + " ( " + classN + ".cs - " + caller.Name + "() )", context);
#endif
    }

    public static void LogError(object errorMessage, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.LogError(errorMessage + " ( " + classN + ".cs - " + caller.Name + "() )", context);
#endif
    }

    public static void LogException(Exception e, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.LogError(e.GetType() + " occured: " + e.Message + " ( " + classN + ".cs - " + caller.Name + "() )",
            context);
#endif
    }

    public static void LogMissing(string name, Object context = null)
    {
        #if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.LogWarning("Following prefab is missing: " + name + " ( " + classN + ".cs - " + caller.Name + "() )",
            context);
        #endif
    }

    public static void DrawRay(Vector3 origin, Vector3 direction, float length, Color color, bool shouldShowInConsole,
        Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.DrawRay(origin, direction * length, color);
        if (shouldShowInConsole)
            Debug.Log(
                "Ray drawn from " + origin + " in the direction " + direction + " ( " + classN + ".cs - " +
                caller.Name + "() )", context);
#endif
    }

    public static void DrawRay(Vector3 origin, Vector3 direction, float length, Color color, LayerMask layerMask,
        bool shouldShowInConsole, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.DrawRay(origin, direction * length, color, layerMask);
        if (shouldShowInConsole)
            Debug.Log(
                "Ray drawn from " + origin + " in the direction " + direction + " ( " + classN + ".cs - " +
                caller.Name + "() )", context);
#endif
    }

    public static void DrawRay(Vector3 origin, Vector3 direction, float length, Color color, float duration,
        bool shouldShowInConsole, Object context = null)
    {
#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        var classN = caller.DeclaringType?.Name;
        Debug.DrawRay(origin, direction * length, color, duration);
        if (shouldShowInConsole)
            Debug.Log(
                "Ray drawn from " + origin + " in the direction " + direction + " ( " + classN + ".cs - " +
                caller.Name + "() )", context);
#endif
    }
}