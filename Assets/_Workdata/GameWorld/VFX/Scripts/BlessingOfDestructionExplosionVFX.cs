using UnityEngine;

public class BlessingOfDestructionExplosionVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfxExplosion;
    [SerializeField] private float animationDuration = 1.0f;
    private void OnEnable()
    {
        BlessingOfDestruction.OnPickUp += PlayExplosionVFX;
    }

    private void OnDisable()
    {
        BlessingOfDestruction.OnPickUp -= PlayExplosionVFX;
    }


    private void PlayExplosionVFX(Vector3 position)
    {
        if (vfxExplosion != null)
        {
            GameObject vfxInstance = Instantiate(vfxExplosion, position, Quaternion.identity);

            Destroy(vfxInstance, animationDuration);
        }
        else
        {
            Debug.LogWarning("Explosion Prefab is not Assigned in Vfx Control System");
        }
    }
}
