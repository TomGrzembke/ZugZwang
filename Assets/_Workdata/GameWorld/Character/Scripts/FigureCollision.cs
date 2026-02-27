using System;
using System.Collections;
using MyBox;
using UnityEngine;

public class FigureCollision : MonoBehaviour
{
    [SerializeField, Tag] private string tagToCheck = "Player";
    [SerializeField] private AutoMove autoMove;
    [SerializeField] private bool CanCollideWhileMoving;
    [SerializeField] private float waitFramesUntilBounce = 1;

    [SerializeField] private float bounceTime = 1;
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private new Collider collider = new();
    
    

    private void Awake()
    {
        collider = GetComponent<Collider>();

        if (!CanCollideWhileMoving)
        {
            DeactivateHitbox();
        }
    }

    private void OnEnable()
    {
        if (CanCollideWhileMoving) return;
        autoMove.OnMoveStart += DeactivateHitbox;
        autoMove.OnMoveEnd += ActivateHitbox;
    }

    private void OnDisable()
    {
        if (CanCollideWhileMoving) return;
        autoMove.OnMoveStart -= DeactivateHitbox;
        autoMove.OnMoveEnd -= ActivateHitbox;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCheck)) return;
        var otherMove = other.gameObject.GetComponent<AutoMove>();

        if (otherMove == null) return;

        StartCoroutine(ExecuteAfterFrames(() => Bounce(otherMove), waitFramesUntilBounce));
    }


    private void Bounce(AutoMove otherMove)
    {
        var direction = otherMove.ComesFromZ - autoMove.ComesFromZ;
        direction = Mathf.Clamp(direction, -1, 1);
        direction = Mathf.RoundToInt(direction);

        autoMove.BounceOff(-direction, bounceTime, bounceCurve);
    }

    IEnumerator ExecuteAfterFrames(Action actionToExecute, float frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        if (actionToExecute != null)
        {
            actionToExecute();
        }
    }

    private void ActivateHitbox()
    {
        collider.enabled = true;
    }

    private void DeactivateHitbox()
    {
        collider.enabled = false;
    }
}