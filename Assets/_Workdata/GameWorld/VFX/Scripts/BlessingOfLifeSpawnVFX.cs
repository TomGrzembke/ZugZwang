using UnityEngine;

public class BlessingOfLifeSpawnVFX : MonoBehaviour //Yona


{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float spawnAnimationDuration = 1.0f;


    private void OnEnable()
    {
        BlessingOfLife.OnFigureSpawnPositionCalculated += PlaySpawnVFX;
    }

    private void OnDisable()
    {
        BlessingOfLife.OnFigureSpawnPositionCalculated -= PlaySpawnVFX;
    }

    private void PlaySpawnVFX(Vector3 positon)
    {
        if (vfxPrefab != null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, positon, Quaternion.identity);

            Destroy(vfxInstance, spawnAnimationDuration);
        }
        else
        {
            Debug.LogWarning("BlessingOfLife Vfx Prefab is not Assigned in the VFXSystem, hi hihi");
        }
    }
}
