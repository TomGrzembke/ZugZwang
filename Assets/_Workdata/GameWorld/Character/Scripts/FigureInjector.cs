using MyBox;
using UnityEngine;

public class FigureInjector : MonoBehaviour
{
    [Separator("Setup")] [SerializeField] private AutoMove autoMove;

    [Separator("Runtime")]
#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField]
    [ReadOnly]
    private string Information = "Used for factory Access";
#pragma warning restore CS0414 // Field is assigned but its value is never used
    [field: SerializeField] public FigureFactory HeritageFactory { get; private set; }
    [SerializeField] private SwipeInput swipeInput;
    private bool spawnedByFactory;
    [SerializeField] private GameObject prefabToPool;

    private void Start()
    {
        if (spawnedByFactory) return;
        if (HeritageFactory == null)
        {
            Debug.LogError("HeritageFactory is null", this);
            return;
        }

        HeritageFactory.AddFigure(transform.parent.gameObject, prefabToPool);
        autoMove.Initialize(HeritageFactory.RoundTimer, swipeInput);
    }

    public void Initialize(FigureFactory figureFactory, SwipeInput swipeInput)
    {
        HeritageFactory = figureFactory;
        if (HeritageFactory == null)
        {
            Debug.LogError("HeritageFactory is null", this);
            return;
        }

        this.swipeInput = swipeInput;
        autoMove.Initialize(HeritageFactory.RoundTimer, swipeInput);
        spawnedByFactory = true;
    }
}