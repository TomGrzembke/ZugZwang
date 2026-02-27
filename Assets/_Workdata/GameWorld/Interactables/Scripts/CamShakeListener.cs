using Unity.Cinemachine;
using UnityEngine;

public class CamShakeListener : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin screenShakeComponent;
    [SerializeField] private float shakeDuration = 0.5f;

    private void OnEnable()
    {
        screenShakeComponent =
            Camera.main.transform.parent.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        BlessingOfDestruction.OnExplosion += CamShake;
    }

    private void OnDisable()
    {
        BlessingOfDestruction.OnExplosion -= CamShake;
    }

    private void CamShake()
    {
        SetShaking(true);
        
        Invoke(nameof(DisableShake), shakeDuration);
    }

    private void DisableShake()
    {
        SetShaking(false);
    }
    
    private void SetShaking(bool condition)
    {
        screenShakeComponent.enabled = condition;
    }
}