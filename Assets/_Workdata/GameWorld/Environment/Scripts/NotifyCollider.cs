using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class NotifyCollider : MonoBehaviour
{
    [Separator("Notification Settings")] [SerializeField] [Tag]
    private string tagToNotify = "Player";

    [SerializeField] private MoveRow moveRow;

    private readonly List<AutoMove> autoMoves = new();

    private void Awake()
    {
        moveRow.OnMoved += NotifySwipe;
        moveRow.OnSink += NotifySink;
    }

    private void OnDestroy()
    {
        moveRow.OnMoved -= NotifySwipe;
        moveRow.OnSink -= NotifySink;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToNotify)) autoMoves.Add(other.GetComponent<AutoMove>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagToNotify))
            if (autoMoves.Contains(other.GetComponent<AutoMove>()))
                autoMoves.Remove(other.GetComponent<AutoMove>());
    }

    public void NotifySwipe(int zOffset)
    {
        autoMoves.ForEach(move => move.SetCurrentOffset(zOffset));
    }
    
    public void NotifySink()
    {
        autoMoves.ForEach(move => move.OnSinkingField());
    }
}