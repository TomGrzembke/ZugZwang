using System.Linq;
using MyBox;
using UnityEngine;

public class SegmentReferences : MonoBehaviour
{
    [field: SerializeField] public FigureFactory FigureFactory { get; private set; }
    [field: SerializeField] public DiveDownDelegator DiveDownDelegator { get; private set; }
    [field: SerializeField] public ObjectPooling ObjectPooling { get; private set; }
    [field: SerializeField] public RoundTimer RoundTimer { get; private set; }
    [field: SerializeField] public HighScore HighScoreObject { get; private set; }
    [field: SerializeField] public CanvasReferences CanvasReferences { get; private set; }

    [Separator("Runtime")]
    [field: SerializeField]
    public int EnvironmentPackID { get; private set; } = -1;

    private void OnEnable()
    {
        Invoke(nameof(Setup), 1);
    }

    private void Setup()
    {
        foreach (var blessing in GetComponentsInChildren<BlessingOfLife>())
        {
            if (FigureFactory != null)
                FigureFactory.OnFigureSpawnedListChanged += blessing.OnFigureCountChanged;
        }

        if (DiveDownDelegator != null)
        {
            DiveDownDelegator.SubscribeRows(GetComponentsInChildren<MoveRow>().ToList());
        }
    }

    public void Cleanup()
    {
        foreach (var blessing in GetComponentsInChildren<BlessingOfLife>())
        {
            if (FigureFactory != null)
                FigureFactory.OnFigureSpawnedListChanged -= blessing.OnFigureCountChanged;
        }

        if (DiveDownDelegator != null)
        {
            DiveDownDelegator.DesubscribeRows(GetComponentsInChildren<MoveRow>().ToList());
        }
    }

    private void OnDisable()
    {
        Cleanup();
    }

    public void SetReferences(FigureFactory factory, DiveDownDelegator delegator, ObjectPooling pooling,
        RoundTimer timer, HighScore highScoreObject, int environmentPackID, CanvasReferences canvas)
    {
        FigureFactory = factory;
        DiveDownDelegator = delegator;
        ObjectPooling = pooling;
        RoundTimer = timer;
        HighScoreObject = highScoreObject;
        EnvironmentPackID = environmentPackID;
        CanvasReferences = canvas;
    }

    public void SetOtherSegmentRef(SegmentReferences segmentReferences)
    {
        segmentReferences.SetReferences(FigureFactory, DiveDownDelegator, ObjectPooling, RoundTimer, HighScoreObject,
            EnvironmentPackID, CanvasReferences);
    }

    public void SetEnvironmentPackID(int packID)
    {
        EnvironmentPackID = packID;
    }
}