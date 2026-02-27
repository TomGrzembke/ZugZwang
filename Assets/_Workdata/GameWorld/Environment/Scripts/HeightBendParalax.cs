using MyBox;
using UnityEngine;

/// <summary>
///     Utilizes the exact same calculation as the rounded world subshader to calculate the perfect position the
///     collider should be in to match visuals
/// </summary>
public class HeightBendParalax : MonoBehaviour
{
    [Tooltip("This will be the parent, or collider object itself commonly")]
    [SerializeField]
    [ConditionalField(nameof(colliderToMove), true)]
    private Transform objectToMove;

    [Tooltip("This will use the collider offset Itself, if you just want to move it and not the object + children")]
    [ConditionalField(nameof(objectToMove), true)]
    [SerializeField]
    private BoxCollider colliderToMove;


    [SerializeField] private float heightParalaxOffset = 0.1f;
    [SerializeField] private float worldBendStrength = 0.001f;

    private Transform camTransform;
    private Vector3 initialColliderCenterPosition;

    private void Awake()
    {
        camTransform = Camera.main.transform;

        if (colliderToMove != null) initialColliderCenterPosition = colliderToMove.center;
    }

    private void Update()
    {
        var targetPos = Vector3.zero;
        if (objectToMove != null) targetPos = objectToMove.position;
        if (colliderToMove != null) targetPos = colliderToMove.transform.position;

        var posDifference = camTransform.position - targetPos;
        var squaredDifference = Mathf.Pow(posDifference.x, 2);
        var difference = squaredDifference * -worldBendStrength;
        var offsetDifference = difference - heightParalaxOffset;

        if (objectToMove != null)
            objectToMove.position = objectToMove.position.ChangeY(offsetDifference);
        else if (colliderToMove != null)
            colliderToMove.center = colliderToMove.center.ChangeY(offsetDifference +
                                                                  colliderToMove.transform.position.y +
                                                                  initialColliderCenterPosition.y);
    }

    public void SetWorldBendStrength(float worldBendStrength)
    {
        this.worldBendStrength = worldBendStrength;
    }
}