using TMPro;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsField;
    [SerializeField] private float hudRefreshRate = 1f;

    private float timer;

    private void Update()
    {
        if(fpsField == null) return;
        
        if (Time.unscaledTime > timer)
        {
            var fps = (int)(1f / Time.unscaledDeltaTime);
            fpsField.text = "FPS: " + fps;
            timer = Time.unscaledTime + hudRefreshRate;
        }
    }
}