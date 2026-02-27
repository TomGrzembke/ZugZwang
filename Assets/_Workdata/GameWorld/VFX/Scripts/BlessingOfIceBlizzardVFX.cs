using UnityEngine;
using UnityEngine.UIElements;

public class BlessingOfIceBlizzardVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfxBlizzardPrefab;
    [SerializeField] private float blizzardAnimationDuration = 1.0f;


    private void OnEnable()
    {
        BlessingOfIce.OnPickUp += PlayBlizzardVFX;
    }

    private void OnDisable()
    {
        BlessingOfIce.OnPickUp -= PlayBlizzardVFX;
    }
    private void PlayBlizzardVFX(Vector3 position)
    {
        if (vfxBlizzardPrefab != null)
        {
            GameObject vfxInstance = Instantiate(vfxBlizzardPrefab, position, Quaternion.identity);

            Destroy(vfxInstance, blizzardAnimationDuration);
        }
        else
        {
            Debug.LogWarning("Blizzard Prefab is not Assigned in the VFX System");
        }
    
    }
}
