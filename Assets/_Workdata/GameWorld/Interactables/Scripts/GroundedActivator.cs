using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

/// <summary> Toggle Gameobjects active when theyre grounded and updates them otherwise</summary>
[RequireComponent(typeof(Collider))]
public class GroundedActivator : MonoBehaviour
{
    [SerializeField, Tag] private string fieldTag;
    [SerializeField] private GameObject[] objectsToToggle;

    private List<Collider> collidersInRange = new List<Collider>();

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        Deactivate();

        col.enabled = false;
    }

    private void Start()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(fieldTag)) return;

        if (!collidersInRange.Contains(other))
        {
            collidersInRange.Add(other);
        }

        if (collidersInRange.Count > 0)
        {
            Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(fieldTag)) return;

        if (collidersInRange.Contains(other))
        {
            collidersInRange.Remove(other);
        }

        if (collidersInRange.Count == 0)
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            obj.SetActive(true);
        }
    }

    private void Deactivate()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            obj.SetActive(false);
        }
    }
}