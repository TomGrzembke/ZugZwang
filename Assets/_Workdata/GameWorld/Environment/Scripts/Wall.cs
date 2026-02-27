using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wall : MonoBehaviour
{
    [Separator("Prefab Settings")] [Space(10)] [SerializeField]
    private GameObject objectToChange;

    [SerializeField] private List<GameObject> prefabsToRandomize;

    public Action OnDestroy;
    public static event Action<Vector3> Explode;
    [SerializeField] private CoinCollectable coinCollectable;


    private GameObject spawnedObject;

    private void Start()
    {
        if (prefabsToRandomize.Count > 0)
        {
            spawnedObject = Instantiate(prefabsToRandomize[Random.Range(0, prefabsToRandomize.Count)],
                objectToChange.transform.position,
                objectToChange.transform.rotation, transform);
            Destroy(objectToChange);
        }
        
        if(spawnedObject == null) return;

        var rand1 = Random.Range(0, 2);

        spawnedObject.transform.localScale = new Vector3(
            spawnedObject.transform.localScale.x,
            spawnedObject.transform.localScale.y,
            rand1 == 0 ? spawnedObject.transform.localScale.z : -spawnedObject.transform.localScale.z
        );
    }

    public void Destroy(float delay)
    {
        Invoke(nameof(Destroy), delay);
        if (coinCollectable != null)
        {
            coinCollectable.Collect();
        }
    }

    private void Destroy()
    {
        OnDestroy?.Invoke();
        Explode?.Invoke(transform.position);
        gameObject.SetActive(false);

    }
}