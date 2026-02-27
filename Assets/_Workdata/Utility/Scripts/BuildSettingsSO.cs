using UnityEngine;

[CreateAssetMenu(fileName = "BuildSettingsSO", menuName = "Scriptable Objects/BuildSettingsSO", order = 0)]
public class BuildSettingsSO : ScriptableObject
{
    [field: SerializeField] public bool ExhibitVersion { get; private set; } = false;
}

