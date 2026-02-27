using UnityEngine;

public class ActAsChildOf : MonoBehaviour
{
    [SerializeField] private Transform positionOf;

    private void Update()
    {
        UpdatePos();
    }

    private void UpdatePos()
    {
        if (positionOf == null) return;
        transform.localPosition = positionOf.localPosition;
    }

    private void OnValidate()
    {
        UpdatePos();
    }
}