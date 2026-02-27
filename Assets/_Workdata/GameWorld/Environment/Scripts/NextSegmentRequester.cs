using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class NextSegmentRequester : MonoBehaviour
{
    [SerializeField] private SegmentSetup segmentSetup;
    [SerializeField] private SegmentReferences segmentReferences;
    [SerializeField] private bool shouldSpawnFields = true;

    [Separator("Prefab & Positioning")] [SerializeField, ConditionalField(nameof(shouldSpawnFields))]
    private GameObject nextFieldPrefab;

#pragma warning disable 0414
    [SerializeField, ReadOnly] private string Info =
        "Will take nextFieldPrefab if theres none provided below, the same segment as this will be ignored!";
#pragma warning restore 0414

    [SerializeField] private List<GameObject> possibleNextSegments;

    private bool SpawnedNextSegment = false;


    private void OnEnable()
    {
        segmentSetup.OnCleanup += Cleanup;
    }

    private void OnDisable()
    {
        segmentSetup.OnCleanup -= Cleanup;
    }

    /// <summary> Instantiates the next Field and passes references and numbers.</summary>
    public void LoadNextSegment()
    {
        StartCoroutine(ExecuteAfterFrames(() => LoadNextSegmentCall(), 1));

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

    public void SafetyLoadNext()
    {
        SpawnedNextSegment = false;
        LoadNextSegmentCall();
    }
    
    public void LoadNextSegmentCall()
    {
        if (!shouldSpawnFields) return;
        if (SpawnedNextSegment) return;
        SpawnedNextSegment = true;

        if (possibleNextSegments.Count > 0)
        {
            GameObject nextSegment = possibleNextSegments[Random.Range(0, possibleNextSegments.Count)];

            for (int i = 0; i < possibleNextSegments.Count; i++)
            {
                nextSegment = possibleNextSegments[Random.Range(0, possibleNextSegments.Count)];
                if (nextSegment != null) break;
            }

            if (nextSegment != null && nextSegment != gameObject)
            {
                nextFieldPrefab = nextSegment;
            }
        }

        var nextSegmentCount = segmentSetup.GetSegmentCount() + 1;

        var nextPos = new Vector3(
            transform.position.x + nextFieldPrefab.GetComponent<SegmentSetup>().FieldLength,
            - segmentSetup.GetSpawnOffsetY(), transform.position.z);

        var nextFieldGO = segmentReferences.ObjectPooling.SpawnObject(nextFieldPrefab, nextPos, Quaternion.identity,
            ObjectPooling.PoolType.SEGMENTS);

        var nextField = nextFieldGO.GetComponent<SegmentSetup>();
        var environmentRandomizer = nextField.GetComponentInChildren<EnvironmentRandomizer>();

        if (nextField == null || environmentRandomizer == null) return;
        
        var shouldReset = environmentRandomizer.SetAssetSegment(GetComponentInChildren<EnvironmentRandomizer>().assetIndex, nextSegmentCount);
        segmentReferences.SetOtherSegmentRef(nextField.References);
        
        if (shouldReset)
            nextField.ResetSegmentCount();
        else
            nextField.StartSpawningNextSegment(nextSegmentCount);
    }

    private void Cleanup()
    {
        SpawnedNextSegment = false;
    }
}