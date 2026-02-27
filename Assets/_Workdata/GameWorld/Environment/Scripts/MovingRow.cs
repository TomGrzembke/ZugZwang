using UnityEngine;

public class MovingRow : MonoBehaviour
{
    [SerializeField, Tooltip("Defines the steps it does during one turn.")]
    private float moveDirectionZ = 1f;

    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private SegmentReferences segmentReferences;
    private MoveRow pushable;

    private RoundTimer timer;

    private GameObject currentVFXInstance;

    private void Awake()
    {
        pushable = GetComponentInChildren<MoveRow>();
    }

    private void Start()
    {
        pushable.DeactivateSwiping();
        pushable.GetComponentInChildren<SwipableVFX>().gameObject.SetActive(false);

        SetRoundTimer(segmentReferences.RoundTimer);
        
        currentVFXInstance = segmentReferences.ObjectPooling.SpawnObject(vfxPrefab, transform.position,
            Quaternion.identity, ObjectPooling.PoolType.VFX);
        currentVFXInstance.transform.parent = transform;
    }


    private void OnDisable()
    {
        if (timer != null)
        {
            timer.OnEarlyTurn -= PushToSide;
        }

        if (segmentReferences.ObjectPooling != null)
        {
            segmentReferences.ObjectPooling.ReturnObjectToPool(currentVFXInstance, ObjectPooling.PoolType.VFX); // Yona
        }
    }

    private void PushToSide()
    {
        pushable.PushToSide(new Vector2(moveDirectionZ, 0));
    }

    public void SetRoundTimer(RoundTimer timer)
    {
        this.timer = timer;
        timer.OnEarlyTurn += PushToSide;
    }

    public RoundTimer GetRoundtimer()
    {
        return timer;
    }

    public void SetSegmentReferenceSetup(SegmentReferences segmentRefs)
    {
        this.segmentReferences = segmentRefs;
    }
}