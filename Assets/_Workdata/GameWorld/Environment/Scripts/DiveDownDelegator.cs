using System.Collections.Generic;
using MyBox;
using UnityEngine;

/// <summary> For scene gathering field dive down approach </summary>
public class DiveDownDelegator : MonoBehaviour
{
    [SerializeField] private RoundTimer roundTimer;
    [SerializeField] private int diveDelay = 2;
    private int currentFieldToDive;
    [SerializeField] private int downDepth = 5;
    [SerializeField] private float moveDownTime = 3f;

    [SerializeField] private int minimumSkipFieldLimit = 7;
    [SerializeField] private FigureFactory figureFactory;
    private ListSorter listSorter = new ListSorter();
    [SerializeField] private FieldScaleSO fieldScaleSo;

    [SerializeField, Tooltip("Will skip 1 turn of dive down all x segments.")]
    private int skipTurnValue = 12;

    private int currentSkipTurn;
    [SerializeField] private AnimationCurve moveDownCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Separator("Runtime")] public List<MoveRow> moveRows;

    private GameObject lastFigure;

    private void Awake()
    {
        roundTimer.OnTurn += NotifyRow;

        figureFactory.OnFigureSpawnedListChanged += CalculateLastFigure;
    }

    private void Start()
    {
        currentFieldToDive = -diveDelay;
    }

    public void SubscribeRows(List<MoveRow> newMoveRows)
    {
        List<GameObject> moveRowObjects = new List<GameObject>();

        foreach (var row in newMoveRows)
        {
            moveRowObjects.Add(row.gameObject);
        }

        moveRowObjects = moveRowObjects.BubbleSortAfterX();
        newMoveRows.Clear();

        foreach (var moveRowObject in moveRowObjects)
        {
            newMoveRows.Add(moveRowObject.GetComponent<MoveRow>());
        }

        moveRows.AddRange(newMoveRows);
    }

    public void DesubscribeRows(List<MoveRow> newMoveRows)
    {
        foreach (var row in newMoveRows)
        {
            if (moveRows.Contains(row))
            {
                moveRows.Remove(row);
                currentFieldToDive--;
            }
        }
    }

    private void OnDestroy()
    {
        roundTimer.OnTurn -= NotifyRow;
    }

    private void NotifyRow(CurrentPhaseProperties phaseSettings)
    {
        currentFieldToDive++;
        if (currentFieldToDive < 0) return;

        var canSkipTurn = lastFigure.transform.position.x - moveRows[currentFieldToDive].transform.position.x <
                          minimumSkipFieldLimit * fieldScaleSo.fieldMultiplier.x;

        if (canSkipTurn)
        {
            currentSkipTurn++;
        }
        
        if (currentSkipTurn >= skipTurnValue)
        {
            currentSkipTurn = 0;
            currentFieldToDive--;
            return;
        }

        if (moveRows.Count <= currentFieldToDive)
        {
            Debugger.Log("Cant dive row", this);
            return;
        }

        moveRows[currentFieldToDive].DiveDown(downDepth, moveDownTime, moveDownCurve);
    }

    private void CalculateLastFigure(List<GameObject> figures)
    {
        lastFigure = listSorter.GetLastXPosition(figures);
    }
}