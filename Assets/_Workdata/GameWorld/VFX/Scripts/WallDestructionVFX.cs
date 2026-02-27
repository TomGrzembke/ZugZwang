using System.Collections.Generic;
using UnityEngine;

public class WallDestructionVFX : MonoBehaviour
{
    [SerializeField] private List<GameObject> vfxDestruction;
    [SerializeField] private float animationDuration = 1.0f;
    private void OnEnable()
    {
        Wall.Explode += PlayDestructionVFX;
    }

    private void OnDisable()
    {
        Wall.Explode -= PlayDestructionVFX;
    }


    private void PlayDestructionVFX(Vector3 position)
    {
        if (vfxDestruction != null && vfxDestruction.Count > 0)
        {
            foreach (GameObject vfx in vfxDestruction)
            {
                GameObject vfxInstance = Instantiate(vfx, position, Quaternion.identity);

                Destroy(vfxInstance, animationDuration);
            }
        }
        else
        {
            Debug.LogWarning("Explosion Prefab is not Assigned in Vfx Control System");
        }
    }
}
