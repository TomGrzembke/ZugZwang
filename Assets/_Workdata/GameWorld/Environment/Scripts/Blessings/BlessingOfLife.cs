using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = System.Random;

public class BlessingOfLife : MonoBehaviour, IBlessing
{
    [Separator("References")]
    [SerializeField] private FieldScaleSO fieldScaleSO;
    [SerializeField] private FieldStatusChecker fieldStatusChecker;

    [Separator("Settings")]
    [SerializeField, Tag] private string tagToCheck = "Player";
    [SerializeField] private bool spawnSpecificFigures;

    [SerializeField, ConditionalField(nameof(spawnSpecificFigures))]
    private GameObject figureToSpawn;
    [SerializeField]
    private int maxBlessingCount = 2;

    [Separator("Events")]
    public Action<Collider> OnCollected;
    public static event Action<Vector3> OnFigureSpawnPositionCalculated;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagToCheck)) return;

        Invoke(nameof(Deactivate), 0.1f);

        OnCollected?.Invoke(other);

        FigureInjector hitInjector = other.GetComponentInChildren<FigureInjector>();
        if (hitInjector == null) return;
        
        FigureFactory factory = hitInjector.HeritageFactory;
        Vector3 spawnPos = GetRandomAdjacentPosition();

        OnFigureSpawnPositionCalculated?.Invoke(spawnPos); // Yona 
        
        if (spawnSpecificFigures && figureToSpawn) 
            factory.SpawnFigure(figureToSpawn, spawnPos);
        else 
            factory.SpawnRandomFigure(spawnPos);
    }

    private Vector3 GetRandomAdjacentPosition()
    {
        List<Vector3> possiblePositions = new();
        Vector3 blessingPos = transform.position;

        for (int i = -4; i < 5; i++)
        {
            if (i == 0) continue;

            var currentCheckPos = blessingPos.ChangeZ(blessingPos.z - fieldScaleSO.fieldMultiplier.z * i);
            var currentCheck = fieldStatusChecker.GetFieldStatus(currentCheckPos);

            if (currentCheck.cantMoveReason == CantMoveReason.CAN_MOVE)
                possiblePositions.Add(currentCheckPos);
        }

        if (possiblePositions.Count < 1)
            return blessingPos;
        
        return possiblePositions[UnityEngine.Random.Range(0, possiblePositions.Count)];
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnFigureCountChanged(List<GameObject> obj)
    {
        if (obj.Count >= maxBlessingCount) 
            gameObject.SetActive(false);
    }
}