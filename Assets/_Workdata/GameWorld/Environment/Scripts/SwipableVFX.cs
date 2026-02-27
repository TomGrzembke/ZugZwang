using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SwipableVFX : MonoBehaviour, ISwipable
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private LayerMask rayAbleLayer;
    [SerializeField] private SegmentReferences segmentReferences;
    private SwipeVFXMonitorer _currentVFXController;
    private MoveRow _thisMoveRow;
    public Action OnFieldSwiped;

    void Awake()
    {
        _thisMoveRow = GetComponentInParent<MoveRow>();
        if (_thisMoveRow == null)
        {
            Debug.LogError("SwipableVFX: MoveRow component not found on this GameObject!", this);
        }
    }

    public void OnSwiped(Vector2 swipeDir)
    {
        if (_thisMoveRow != null)
        {
        }
        else
        {
            Debug.LogWarning("SwipableVFX: Cannot swipe, no MoveRow component found to control movement.", this);
        }


        if (_currentVFXController != null)
        {
            _currentVFXController.StartVFXMovement(swipeDir, _thisMoveRow.GetMoveTime(), _thisMoveRow.GetMoveCurve());
        }
    }

    public void OnSelect(Ray hitRay)
    {
        if (vfxPrefab == null) return;

        RaycastHit hit;
        if (!Physics.Raycast(hitRay, out hit, 100f, rayAbleLayer)) return;


        OnFieldSwiped?.Invoke();


        if (Time.timeScale == 0) return;
        GameObject newVFXInstance =
            segmentReferences.ObjectPooling.SpawnObject(vfxPrefab, transform.position, Quaternion.identity,
                ObjectPooling.PoolType.VFX);

        //GameObject newVFXInstance = Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        SwipeVFXMonitorer vfxController = newVFXInstance.GetComponent<SwipeVFXMonitorer>();

        if (vfxController != null)
        {
            _currentVFXController = vfxController;

            if (_thisMoveRow != null)
            {
                vfxController.SetTargetMoveRow(_thisMoveRow);
            }

            vfxController.StartVFXLifecycle(segmentReferences.ObjectPooling);
        }
    }

    public void OnDeselect(Ray hit)
    {
        if (_currentVFXController != null)
        {
            _currentVFXController.StartFadeOut();
        }
    }
    
    public void SetSegmentRefSetup(SegmentReferences segmentReferences)
    {
        this.segmentReferences = segmentReferences;
    }
}