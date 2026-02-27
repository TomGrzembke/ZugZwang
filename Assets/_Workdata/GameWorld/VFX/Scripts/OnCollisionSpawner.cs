using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
public class OnCollisionSpawner : MonoBehaviour

{
    [SerializeField] private GameObject impactVFXPrefab;
    [Tooltip("The tag of the object that will trigger the particles. If left empty every object with collision is a trigger")]
    [SerializeField] private string triggerTag = "";
    private HashSet<GameObject> triggeredObjects = new HashSet<GameObject>();
    [SerializeField] private ObjectPooling objectPooling;
    [SerializeField] private SegmentReferences segmentReferences;


    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag))
        {
            return;
        }

        if (triggeredObjects.Contains(other.gameObject))
        {
            return;
        }

        triggeredObjects.Add(other.gameObject);

        if (impactVFXPrefab != null)
        {

            GameObject newImpactVFX = GetPool().SpawnObject(impactVFXPrefab, other.transform.position, Quaternion.identity, ObjectPooling.PoolType.VFX);

            ParticleSystem[] particleSystems = newImpactVFX.GetComponentsInChildren<ParticleSystem>();


            foreach (ParticleSystem ps in particleSystems)
            {
                var mainModule = ps.main;
                mainModule.loop = false;

                ps.Play();
            }

            StartCoroutine(WaitForParticlesToDie(newImpactVFX, particleSystems));

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (triggeredObjects.Contains(other.gameObject))
        {
            triggeredObjects.Remove(other.gameObject);
        }
    }
    private IEnumerator WaitForParticlesToDie(GameObject vfxGameObject, ParticleSystem[] particleSystems)
    {

        while (particleSystems.Any(ps => ps.IsAlive(true)))
        {
            yield return null;
        }

        if (vfxGameObject != null)
        {
            GetPool().ReturnObjectToPool(vfxGameObject, ObjectPooling.PoolType.VFX);
        }
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
