using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectRatio : MonoBehaviour
{
    [SerializeField] private Vector2 ratio = new(9, 16);
    [SerializeField] private bool use;
    private Camera _camera;
    private float targetAspectRatio => ratio.x / ratio.y; // The desired aspect ratio, e.g., 16:9


    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        if (use)
            SetCameraAspect();
    }

    private void Update()
    {
       // if (use)
          //  SetCameraAspect();
    }


    private void SetCameraAspect()
    {
        var windowAspect = (float)Screen.width / Screen.height;
        var scaleHeight = windowAspect / targetAspectRatio;

        if (scaleHeight < 1.0f)
        {
            // Letterboxing
            var rect = _camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            _camera.rect = rect;
        }
        else
        {
            // Pillarboxing
            var scaleWidth = 1.0f / scaleHeight;

            var rect = _camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            _camera.rect = rect;
        }
    }

    public void SetResX(float x)
    {
        ratio.x = x;
    }

    public void SetResY(float y)
    {
        ratio.y = y;
    }
}