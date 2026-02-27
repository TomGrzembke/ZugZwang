using System;
using System.Collections;
using MyBox;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Separator("References")] [SerializeField]
    private FieldScaleSO fieldScaleSO;

    private CoroutineOperation coroutineOperation;
    private int currentOffset;
    private float initialY;
    private CoroutineOperation loopTimeOperation;

    private void Start()
    {
        currentOffset = Mathf.RoundToInt(transform.position.z / fieldScaleSO.fieldMultiplier.z);
        initialY = transform.localPosition.y;
    }

    public event Action<int> OnLoop;
    public event Action<int> OnMove;
    public event Action<int> OnMoveFinished;

    public void AddOffset(int addedOffset, float moveTime, AnimationCurve moveCurve = null)
    {
        currentOffset += addedOffset;
        OnMove?.Invoke(addedOffset * (int)fieldScaleSO.fieldMultiplier.z);
        
        if(!gameObject.activeInHierarchy) return;
        StartCoroutine(MoveOffsetCor(moveTime, moveCurve));
    }

    public void AddOffsetWithLoop(int addedOffset, float moveTime, int segmentThickness,
        AnimationCurve moveCurve = null)
    {
        currentOffset += addedOffset;
        var currentSegmentThickness = segmentThickness + 1;
        var halfSegmentThickness = segmentThickness / 2;

        if (currentOffset < -halfSegmentThickness)
            currentOffset += currentSegmentThickness;
        else if (currentOffset > halfSegmentThickness) currentOffset -= currentSegmentThickness;

        OnMove?.Invoke(addedOffset * (int)fieldScaleSO.fieldMultiplier.z);
        StartCoroutine(MoveOffsetCor(moveTime, moveCurve));
    }

    private IEnumerator MoveOffsetCor(float moveTime, AnimationCurve moveCurve = null)
    {
        var timeWentBy = 0f;
        var startOffset = currentOffset;

        if (moveCurve == null) moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

        var initialZ = transform.localPosition.z;

        //Movement part
        while (moveTime > timeWentBy)
        {
            if (startOffset != currentOffset) yield break;

            timeWentBy += Time.deltaTime;

            transform.localPosition =
                transform.localPosition.ChangeZ(CalculateStep(initialZ, currentOffset * fieldScaleSO.fieldMultiplier.z,
                    timeWentBy, moveTime, moveCurve));

            yield return null;
        }

        OnMoveFinished?.Invoke(currentOffset);
    }

    public CoroutineOperation MoveUpDown(float depth, float moveTime1, AnimationCurve moveCurve1, float moveTime2,
        AnimationCurve moveCurve2)
    {
        var currentDownTime = moveTime1;
        var currentUpTime = moveTime2;

        // Stops field going up on the side when swiped back
        if (loopTimeOperation != null && !loopTimeOperation.IsFinished)
        {
            loopTimeOperation.StopCoroutine();
            currentDownTime /= 5;
            currentUpTime /= 2;
        }
        
        loopTimeOperation = new CoroutineOperation(this,
            MoveUpDownCor(depth, moveTime1, moveCurve1, currentUpTime, moveCurve2), currentDownTime + currentUpTime);
        return loopTimeOperation;
    }
    
    public void MoveY(float depth, float moveTime, AnimationCurve moveCurve)
    {
        StartCoroutine(MoveYCor(depth, moveTime, moveCurve));
    }

    public void LoopOver(int side, float downDepth, float downTime, AnimationCurve downCurve, float upTime,
        AnimationCurve upCurve, float sideMoveTime, int segmentThickness,
        AnimationCurve sideCurve = null)
    {
        MoveUpDown(downDepth, downTime, downCurve, upTime, upCurve);
        AddOffsetWithLoop(-side, sideMoveTime, segmentThickness, sideCurve);

        OnLoop?.Invoke(-side);
    }

    private IEnumerator MoveUpDownCor(float depth, float moveTime1, AnimationCurve moveCurve1, float moveTime2,
        AnimationCurve moveCurve2)
    {
        //var currentY = transform.localPosition.y;

        yield return MoveYCor(initialY -depth, moveTime1, moveCurve1);
        yield return MoveYCor(initialY, moveTime2, moveCurve2);
        
       transform.localPosition = transform.localPosition.ChangeY(initialY);
    }

    private IEnumerator MoveYCor(float  depth, float moveTime, AnimationCurve moveCurve)
    {
        var timeWentBy = 0f;

        var currentY = transform.localPosition.y;

        //Movement part
        while (moveTime > timeWentBy)
        {
            timeWentBy += Time.deltaTime;

            transform.localPosition =
                transform.localPosition.ChangeY(CalculateStep(currentY, depth, timeWentBy, moveTime, moveCurve));

            yield return null;
        }
        
        transform.localPosition =
            transform.localPosition.ChangeY(CalculateStep(currentY, depth, 1, 1, moveCurve));
    }

    private float CalculateStep(float initialValue, float targetValue, float timeWentBy, float moveTime,
        AnimationCurve moveCurve)
    {
        var currentValue = Mathf.Lerp(initialValue, targetValue, moveCurve.Evaluate(timeWentBy / moveTime));

        return currentValue;
    }

    private Vector3 CalculateXStep(float timeWentBy, float moveTime, float initialValue, float targetValue,
        AnimationCurve moveCurve)
    {
        var currentValue = CalculateStep(timeWentBy, moveTime, initialValue, targetValue, moveCurve);

        return new Vector3(currentValue, 0, 0);
    }

}