using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DebugMode))]
public class DebugModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var debugMode = (DebugMode)target;

        if (!Application.isPlaying)
        {
            debugMode.UpdateEditorValues();
            EditorUtility.SetDirty(debugMode);
        }
    }
}
#endif