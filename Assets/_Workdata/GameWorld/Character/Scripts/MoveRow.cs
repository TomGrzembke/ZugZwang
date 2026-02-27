using System.Collections; // Yona: für IEnumerator
using System;
using System.Collections.Generic;
using MyBox;
using UnityEditor;
using UnityEngine;

public class MoveRow : MonoBehaviour, ISwipable
{
    [Separator("Move Options")] [SerializeField]
    private bool isSwipable = true;

    [SerializeField] private float moveTime = 1;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Space(10)] [Separator("Move Elements")] [SerializeField]
    private MoveObject rowDiveDownObject;

    [SerializeField] private Transform fieldsObject;
    [SerializeField] private List<MoveObject> rowElements;

    public event Action<int> OnMoved;
    public event Action OnSink;
    public event Action OnSinkFinished;
    public event Action OnMovementFinished; // Yona: für VFXController
    private float divedDownDepth;
    

    public float GetMoveTime() // Yona: SwipeableVFX braucht das
    {
        return moveTime;
    }

    public AnimationCurve GetMoveCurve()
    {
        return moveCurve;
    } // Yona Ende


    public void OnSwiped(Vector2 swipeDir)
    {
        if(!isSwipable) return;
        PushToSide(swipeDir);
    }

    public void PushToSide(Vector2 swipeDir)
    {
        OnMoved?.Invoke((int)swipeDir.x);

        foreach (var element in rowElements)
        {
            if (element == null) continue;
            element.AddOffset((int)swipeDir.x, moveTime, moveCurve);
        }

        StartCoroutine(WaitForMoveComplete(moveTime)); // Yona: für VFXController (in VFX/Scripts)
    }

    private IEnumerator WaitForMoveComplete(float duration) // Yona
    {
        yield return new WaitForSeconds(duration); // Yona
        OnMovementFinished?.Invoke(); // Yona
    }

    public void OnSelect(Ray ray)
    {
    }

    public void OnDeselect(Ray hit)
    {
    }


    public void DiveDown(float depth, float moveTime, AnimationCurve moveCurve)
    {
        OnSink?.Invoke();

        rowDiveDownObject.MoveY(-depth, moveTime, moveCurve);
        divedDownDepth = depth;

        StartCoroutine(DoAfterTime(() => OnSinkFinished?.Invoke(), moveTime));
    }

    public void Reset()
    {
        rowDiveDownObject.MoveY(0, 0, moveCurve);
    }

    private IEnumerator DoAfterTime(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);
        action.Invoke();
    }

    public void DeactivateSwiping()
    {
        isSwipable = false;
    }

#if UNITY_EDITOR
    /// <summary>The difference this command did will be used in the overarching field collector script</summary>
    /// <returns> The Before and After amounts of the fields</returns>
    [ButtonMethod]
    public Vector2 CollectFields()
    {
        Vector2 fieldBeforeAfter;
        fieldBeforeAfter.x = rowElements.Count;

        rowElements.Clear();
        rowElements.AddRange(fieldsObject.GetComponentsInChildren<MoveObject>());
        if (rowElements.Contains(rowDiveDownObject))
        {
            rowElements.Remove(rowDiveDownObject);
        }

        fieldBeforeAfter.y = rowElements.Count;

        EditorUtility.SetDirty(gameObject);

        return fieldBeforeAfter;
    }
#endif
}