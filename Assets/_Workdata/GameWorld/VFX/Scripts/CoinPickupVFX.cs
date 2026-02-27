using UnityEngine;
using System.Collections;


public class CoinPickupVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private ObjectPooling objectPooling;
    [SerializeField] private SegmentReferences segmentReferences;
    void OnEnable()
    {
        CoinCollectable.OnPickUp += PlayVFX;
    }

    void OnDisable()
    {
        CoinCollectable.OnPickUp -= PlayVFX;
    }

    private void PlayVFX(Vector3 position)
    {
        if (vfxPrefab == null)
        {
            Debug.LogWarning("VFX Prefab reference is missing in CoinPickupVFX.", this);
            return;
        }

        var pool = GetPool();
        GameObject newVFXInstance = pool.SpawnObject(vfxPrefab, position, Quaternion.identity, ObjectPooling.PoolType.VFX);

        newVFXInstance.SetActive(true);

        StartCoroutine(WaitForDeathThenPool(newVFXInstance));

    }

    private IEnumerator WaitForDeathThenPool(GameObject vfxInstance)
    {
        if (vfxInstance == null || !vfxInstance.activeInHierarchy)
        {
            Debug.LogWarning("VFX instance is null or inactive when WaitForDeathThenPool started.", this);
            yield break; // Exit the Coroutine
        }

        ParticleSystem[] particleSystems = vfxInstance.GetComponentsInChildren<ParticleSystem>();

        if (particleSystems.Length == 0)
        {
            vfxInstance.SetActive(false);

            GetPool().ReturnObjectToPool(vfxInstance, ObjectPooling.PoolType.VFX);

            yield break;
        }

        bool anyParticlesAlive = true;
        while (anyParticlesAlive)
        {
            anyParticlesAlive = false;

            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps != null && ps.IsAlive(true))
                {
                    anyParticlesAlive = true;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        vfxInstance.SetActive(false);
        GetPool().ReturnObjectToPool(vfxInstance, ObjectPooling.PoolType.VFX);

    }

     private ObjectPooling GetPool()
    {
        if (objectPooling != null)
        {
            return objectPooling;
        }

        else if (segmentReferences != null)
        {
            return segmentReferences.ObjectPooling;
        }

        else
        {
            return null;
        }
    }

}
