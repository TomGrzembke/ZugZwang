using MyBox;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Separator("Camera Settings")] [SerializeField]
    private Camera cam;

#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField] [ReadOnly] private string Information = "Will take main Camera if 'None' is selected.";
#pragma warning restore CS0414 // Field is assigned but its value is never used

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(cam.transform);
    }
}