using System;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(MoveObject))]
public class SegmentSetup : MonoBehaviour
{
    [SerializeField] float SpawnDuration = 3f;
    [SerializeField] private float SpawnOffsetY = 10f;
    [SerializeField] private MoveObject moveUpObj;

    [PositiveValueOnly]
    [field: SerializeField]
    public int FieldLength { get; private set; } = 36;

    [Separator("Object References")]
    [field: SerializeField]
    public SegmentReferences References { get; private set; }


    [Separator("Spawning")] [SerializeField]
    private AnimationCurve spawnCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Separator("Despawn")] [SerializeField]
    private MoveRow despawnRow;

    public event Action OnCleanup;

    [SerializeField] private int currentSegmentCount;

    private void OnEnable()
    {
        if (despawnRow != null)
        {
            despawnRow.OnSinkFinished += ClearFields;
        }
    }

    private void OnDisable()
    {
        if (despawnRow != null)
        {
            despawnRow.OnSinkFinished -= ClearFields;
        }
    }

    public void ClearFields()
    {
        OnCleanup?.Invoke();
        var moveRows = GetComponentsInChildren<MoveRow>();

        foreach (var moveRow in moveRows)
        {
            moveRow.Reset();
        }

        References.Cleanup();
        References.ObjectPooling.ReturnObjectToPool(gameObject, ObjectPooling.PoolType.SEGMENTS);
    }

    public void ResetSegmentCount()
    {
        currentSegmentCount = 0;
        StartSpawningNextSegment(0);
    }


    public void StartSpawningNextSegment(int nextSegmentCount)
    {
        currentSegmentCount = nextSegmentCount;
        moveUpObj.MoveY(transform.position.y + SpawnOffsetY, SpawnDuration, spawnCurve);
    }

    public int GetSegmentCount()
    {
        return currentSegmentCount;
    }

    public float GetSpawnOffsetY()
    {
        return SpawnOffsetY;
    }
}