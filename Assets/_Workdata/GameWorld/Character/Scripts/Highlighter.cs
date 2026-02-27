using System;
using System.Collections;
using MyBox;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    [Separator("Highlight Fields")] [SerializeField]
    private GameObject highlightObject;

    [SerializeField] private GameObject blockObject;

    [SerializeField] private GameObject possibleDeathObject;

    [SerializeField] private Vector3 highlightOffset;

    [SerializeField] private FigureInjector figureInjector;

    [Separator("References")] [SerializeField]
    private AutoMove autoMoveForHighlights;

    private GameObject currentHighlightGO;
    private GameObject currentBlockHighlightGO;
    private GameObject currentLavaHighlightGO;
    private ObjectPooling objectPooling;

    private void Awake()
    {
        autoMoveForHighlights.OnIntent += Highlight;
        autoMoveForHighlights.OnBlocked += BlockHighlight;
        autoMoveForHighlights.OnLavaIntent += LavaHighlight;
        autoMoveForHighlights.OnDeath += ClearHighlight;
    }


    private void Start()
    {
        objectPooling = figureInjector.HeritageFactory.ObjectPooling;
    }

    private void OnDestroy()
    {
        ClearLastHighlight();

        autoMoveForHighlights.OnLavaIntent -= LavaHighlight;
        autoMoveForHighlights.OnIntent -= Highlight;
        autoMoveForHighlights.OnBlocked -= BlockHighlight;
        //if (currentHighlightGO != null) Destroy(currentHighlightGO);
    }

    private void Highlight(Vector3 position)
    {
        ClearLastHighlight();

        if (objectPooling == null)
        {
            StartCoroutine(DelayAction1Frame(() => Highlight(position)));
            return;
        }

        if (currentHighlightGO == null)
            currentHighlightGO = objectPooling.SpawnObject(highlightObject, position, Quaternion.identity,
                ObjectPooling.PoolType.VFX);

        var spawnPosition = position + highlightOffset;
        spawnPosition.y = 1;

        currentHighlightGO.transform.position = spawnPosition;
        currentHighlightGO.SetActive(true);
    }

    private IEnumerator DelayAction1Frame(Action action)
    {
        yield return null;
        action.Invoke();
    }

    private void BlockHighlight(Vector3 position)
    {
        ClearLastHighlight();

        if (objectPooling == null)
        {
            StartCoroutine(DelayAction1Frame(() => BlockHighlight(position)));
            return;
        }

        if (!currentBlockHighlightGO)
            currentBlockHighlightGO =
                objectPooling.SpawnObject(blockObject, position, Quaternion.identity, ObjectPooling.PoolType.VFX);

        var spawnPosition = position + highlightOffset;
        spawnPosition = spawnPosition.ChangeY(Mathf.Round(spawnPosition.y));

        currentBlockHighlightGO.transform.position = spawnPosition;
        currentBlockHighlightGO.SetActive(true);
    }

    private void LavaHighlight(Vector3 position)
    {
        ClearLastHighlight();
        
        if (objectPooling == null)
        {
            StartCoroutine(DelayAction1Frame(() => LavaHighlight(position)));
            return;
        }

        if (!currentLavaHighlightGO)
            currentLavaHighlightGO =
                objectPooling.SpawnObject(possibleDeathObject, position, Quaternion.identity, ObjectPooling.PoolType.VFX);

        var spawnPosition = position + highlightOffset;
        spawnPosition = spawnPosition.ChangeY(Mathf.Round(spawnPosition.y));

        currentLavaHighlightGO.transform.position = spawnPosition;
        currentLavaHighlightGO.SetActive(true);
    }

    private void ClearLastHighlight()
    {
        if (currentHighlightGO) currentHighlightGO.SetActive(false);

        if (currentBlockHighlightGO)
        {
            currentBlockHighlightGO.SetActive(false);
            objectPooling.ReturnObjectToPool(currentBlockHighlightGO, ObjectPooling.PoolType.VFX);
        }

        if (currentLavaHighlightGO)
        {
            currentLavaHighlightGO.SetActive(false);
            objectPooling.ReturnObjectToPool(currentLavaHighlightGO, ObjectPooling.PoolType.VFX);

        }
    }

    public void ClearHighlight()
    {
        ClearLastHighlight();
    }
}