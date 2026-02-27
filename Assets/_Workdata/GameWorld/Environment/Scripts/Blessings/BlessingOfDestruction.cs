using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class BlessingOfDestruction : MonoBehaviour, IBlessing
{
    [FormerlySerializedAs("fieldLayer")][FormerlySerializedAs("wallLayer")][SerializeField] private LayerMask rowLayer;
    [SerializeField, Tag] private string playerTag = "Player";
    [SerializeField] private float xTimeDifference = 0.2f;

    [Separator("Field Settings")]
    [SerializeField] private int rowsToOverwrite = 5;
    [SerializeField] private float waitForExplosion = 1.0f; //Yona
    private bool hasBeenTriggered = false; //Y
    public static event Action<Vector3> OnPickUp;
    public static event Action OnExplosion;


    public void Reset() //Y
    {
        hasBeenTriggered = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return; //Y
        if (!other.CompareTag(playerTag)) return;

        hasBeenTriggered = true;

        OnPickUp?.Invoke(transform.position); 
        
        foreach (Transform child in transform) //Y
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(WaitForExplosion(waitForExplosion)); //Y
    }

    private List<GameObject> GetFollowingRows()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y += 3;
        List<GameObject> rows = new List<GameObject>();

        for (int i = 0; i < rowsToOverwrite; i++)
        {
            if (Physics.Raycast(currentPosition, Vector3.down, out var hit, 100f, rowLayer))
            {
                GameObject row = hit.collider.gameObject;
                rows.Add(row);
            }

            currentPosition.x += 3;
        }

        return rows;
    }

    private IEnumerator WaitForExplosion(float wait) //Y
    {

        yield return new WaitForSeconds(wait);

        OnExplosion?.Invoke();

        List<GameObject> rows = GetFollowingRows();
        foreach (GameObject row in rows)
        {
            foreach (Wall wall in row.GetComponentsInChildren<Wall>())
            {
                var delay = (transform.position.x - wall.transform.position.x) * xTimeDifference;
                wall.Destroy(Mathf.Abs(delay));
            }
        }

        gameObject.SetActive(false);
    }
}