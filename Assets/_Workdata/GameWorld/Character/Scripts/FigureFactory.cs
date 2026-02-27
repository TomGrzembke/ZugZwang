using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FigureType
{
    NONE = 0,
    BISHOP = 10,
    KNIGHT = 20,
    ROOK = 30,
}

public class FigureFactory : MonoBehaviour
{
    [Separator("Setup")]
#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField]
    [ReadOnly]
    private string Information = "Need to be in Order of the Figurtype Enum.";
#pragma warning restore CS0414 // Field is assigned but its value is never used
    [SerializeField] private List<GameObject> possibleFigurePrefabs;
    [field: SerializeField] public RoundTimer RoundTimer { get; private set; }
    [field: SerializeField] public ObjectPooling ObjectPooling { get; private set; }

    [SerializeField] private SwipeInput swipeInput;


    [Separator("Runtime")]
    [field: SerializeField]
    public List<GameObject> CurrentFigureList { get; private set; } = new();

    public int FigureCount => CurrentFigureList.Count;

    public Action<GameObject /*New Spawned Figure*/> OnFigureSpawned;
    public Action<List<GameObject /*List of Current Figures*/>> OnFigureSpawnedListChanged;

    public void SpawnRandomFigure(Vector3 position)
    {
        SpawnFigure(possibleFigurePrefabs[Random.Range(0, possibleFigurePrefabs.Count)], position);
    }

    public void SpawnFigure(FigureType figureType, Vector3 position)
    {
        switch (figureType)
        {
            case FigureType.ROOK:
                SpawnFigure(possibleFigurePrefabs[0], position);
                break;
            case FigureType.BISHOP:
                SpawnFigure(possibleFigurePrefabs[1], position);
                break;
            case FigureType.KNIGHT:
                SpawnFigure(possibleFigurePrefabs[2], position);
                break;
            default:
                Debugger.Log("Invalid figure type.");
                break;
        }
    }

    public void SpawnFigure(GameObject figure, Vector3 position)
    {
        //var newFigure = Instantiate(figure, position, Quaternion.identity);
        var newFigure = ObjectPooling.SpawnObject(figure, position, Quaternion.identity, ObjectPooling.PoolType.GAMEOBJECTS);
        AddFigure(newFigure);
        newFigure.GetComponentInChildren<FigureInjector>().Initialize(this, swipeInput);
    }


    /// <returns> Number of Figures after call</returns>
    public int RemoveFigure(GameObject figure)
    {
        if (!CurrentFigureList.Contains(figure)) return CurrentFigureList.Count;

        CurrentFigureList.Remove(figure);
        OnFigureSpawnedListChanged?.Invoke(CurrentFigureList);

        ObjectPooling.ReturnObjectToPool(figure, ObjectPooling.PoolType.GAMEOBJECTS);

        return CurrentFigureList.Count;
    }

    /// <returns> Number of Figures after call</returns>
    public int AddFigure(GameObject figure, GameObject prefabToPool = null)
    {
        if (CurrentFigureList.Contains(figure))
        {
            Debug.Log("Figure already exists.", this);
            return CurrentFigureList.Count;
        }

        CurrentFigureList.Add(figure);
        OnFigureSpawnedListChanged?.Invoke(CurrentFigureList);
        OnFigureSpawned?.Invoke(figure);

        if (prefabToPool != null)
        {
            ObjectPooling.AddExisting(figure, prefabToPool, ObjectPooling.PoolType.GAMEOBJECTS);
        }

        return CurrentFigureList.Count;
    }
}