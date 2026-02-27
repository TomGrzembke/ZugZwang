using UnityEngine;

[CreateAssetMenu(fileName = "FieldScaleSO", menuName = "Scriptable Objects/FieldScaleSO", order = 0)]
public class FieldScaleSO : ScriptableObject
{
    /// <summary> z should always be 1 in scale</summary>
    public Vector3 fieldMultiplier = Vector3.one;

    private void OnValidate()
    {
        fieldMultiplier.y = 1;
    }
}