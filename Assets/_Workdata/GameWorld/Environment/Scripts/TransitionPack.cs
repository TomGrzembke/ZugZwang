using System;
using System.Collections;
using UnityEngine;

public class TransitionPack : MonoBehaviour
{
    [SerializeField] private GameObject transitionPack;
    [SerializeField] private SegmentSetup segmentSetup;

    private GameObject spawnedAsset;

    public void SetSegmentSetup(SegmentSetup segment)
    {
        segmentSetup = segment;
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

    private void OnEnable()
    {
        segmentSetup.OnCleanup += Cleanup;
        StartCoroutine(ExecuteAfterFrames(() => Activate(), 1));
    }

    private void OnDisable()
    {
        segmentSetup.OnCleanup -= Cleanup;
    }

    public void Cleanup()
    {
        segmentSetup.References.ObjectPooling.ReturnObjectToPool(spawnedAsset, ObjectPooling.PoolType.GAMEOBJECTS);
    }

    private void Activate()
    {
        if (transitionPack == null)
        {
            Debugger.LogError($"{nameof(transitionPack)}  doesnt contain assets at" + name, this);
            return;
        }

        spawnedAsset = segmentSetup.References.ObjectPooling.SpawnObject(transitionPack, transform.position,
            Quaternion.identity, ObjectPooling.PoolType.GAMEOBJECTS);
        spawnedAsset.transform.SetParent(transform);
    }
}