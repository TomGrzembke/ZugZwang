using System.Collections;
using System.Threading;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private const int MAX_RATE = 99999;
    private float currentFrameTime;
    private readonly float targetFrameRate = 60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MAX_RATE;

        currentFrameTime = Time.realtimeSinceStartup;

        StartCoroutine(WaitForNextFrame());
    }

    private IEnumerator WaitForNextFrame()
    {
        yield return new WaitForEndOfFrame();
        currentFrameTime += 1.0f / targetFrameRate;
        var t = Time.realtimeSinceStartup;
        var sleepTime = currentFrameTime - t - .01f;
        if (sleepTime > 0) Thread.Sleep((int)(sleepTime * 1000));

        while (t < currentFrameTime) t = Time.realtimeSinceStartup;
    }
}