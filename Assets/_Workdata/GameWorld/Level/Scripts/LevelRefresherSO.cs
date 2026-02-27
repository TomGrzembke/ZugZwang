using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelRefresherSO", order = 1)]
public class LevelRefresherSO : ScriptableObject
{
    [ButtonMethod(ButtonMethodDrawOrder.BeforeInspector)]
    public void RefreshLevel()
    {
#if UNITY_EDITOR

        foreach (var segment in segments)
        {
            segment.GetComponentsInChildren<FieldCollector>()
                .FirstOrDefault(fieldCollector => fieldCollector.gameObject != segment).DoAllActions();
            EditorUtility.SetDirty(segment);
        }

        AssetDatabase.SaveAssets();
#endif
    }

#pragma warning disable CS0414 // Field is assigned but its value is never used
    [ReadOnly, SerializeField] private string tooltip = "Only works when out of preview mode!!";
#pragma warning restore CS0414 // Field is assigned but its value is never used
    [field: SerializeField] public List<GameObject> segments { get; private set; }
}