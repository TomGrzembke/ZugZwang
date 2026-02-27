using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AssetSetup : MonoBehaviour
{
    [SerializeField] private List<GameObject> assetPacksList;
    [SerializeField] private SegmentSetup segmentSetup;
    private SegmentReferences segmentReferences;

    private GameObject spawnedAsset;

    public void SetSegmentSetup(SegmentSetup segment)
    {
        segmentSetup = segment;
    }

    private void Awake()
    {
        segmentReferences = segmentSetup.References;
    }

    private void OnEnable()
    {
        segmentSetup.OnCleanup += Cleanup;
        //Invoke(nameof(ActivateRandom), 1);
        StartCoroutine(ExecuteAfterFrames(() => ActivateRandom(), 1));
    }

    IEnumerator ExecuteAfterFrames(Action actionToExecute, float frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        if (actionToExecute != null)
        {
            actionToExecute();
        }
    }

    private void OnDisable()
    {
        segmentSetup.OnCleanup -= Cleanup;
    }

    public void Cleanup()
    {
        if(!gameObject.activeInHierarchy) return;
        
        segmentSetup.References.ObjectPooling.ReturnObjectToPool(spawnedAsset, ObjectPooling.PoolType.GAMEOBJECTS);
    }

    private void ActivateRandom()
    {
        var randomID = Random.Range(0, assetPacksList.Count);

        if (assetPacksList.Count == 0)
        {
            Debugger.LogError($"{nameof(assetPacksList)}  doesnt contain assets at" + name, this);
            return;
        }

        if (assetPacksList.Count == 1)
        {
            randomID = 0;
        }
        else if (randomID == segmentReferences.EnvironmentPackID)
        {
            ActivateRandom();
            return;
        }


        spawnedAsset = segmentSetup.References.ObjectPooling.SpawnObject(assetPacksList[randomID], transform.position,
            Quaternion.identity,
            ObjectPooling.PoolType.GAMEOBJECTS);
        spawnedAsset.transform.SetParent(transform);


        segmentReferences.SetEnvironmentPackID(randomID);
    }
}