using MyBox;
using UnityEngine;

public struct MoveStatus
{
    public Vector3? hinderPosition;
    public Vector3 checkPosition;
    public CantMoveReason cantMoveReason;
    public bool CanMoveTo => cantMoveReason == CantMoveReason.CAN_MOVE || cantMoveReason == CantMoveReason.LAVA_FIELD;

    public MoveStatus(Vector3 checkPosition,Vector3? hinderPosition, CantMoveReason cantMoveReason)
    {
        this.hinderPosition = hinderPosition;
        this.cantMoveReason = cantMoveReason;
        this.checkPosition = checkPosition;
    }
}

public enum CantMoveReason
{
    NONE = 0,
    ALREADY_THERE = 10,
    CAN_MOVE = 20,
    WALL = 30,
    FIELD_LIMIT = 40,
    LAVA_FIELD = 50
}

public class FieldStatusChecker : MonoBehaviour
{
    [SerializeField, Tag] private string wallTag = "Wall";
    [SerializeField, Tag] private string lavaFieldTag = "LavaField";
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Color debugColor = Color.magenta;
    [SerializeField] private float checkDepth = 100f;

    private const int FIELD_SIZE = 5;
    
    public MoveStatus GetFieldStatus(Vector3 position)
    {
        MoveStatus moveStatus = new();
        moveStatus.hinderPosition = position;
        moveStatus.checkPosition = position;
        
        var obj = GetFieldObject(position);
        if (obj == null || position.z < -FIELD_SIZE || position.z > FIELD_SIZE)
        {
            moveStatus.cantMoveReason = CantMoveReason.FIELD_LIMIT;
            return moveStatus;
        }

        if (obj.CompareTag(wallTag))
        {
            moveStatus.cantMoveReason = CantMoveReason.WALL;
            return moveStatus;
        }

        if (obj.CompareTag(lavaFieldTag))
        {
            moveStatus.cantMoveReason = CantMoveReason.LAVA_FIELD;

            return moveStatus;
        }
        
        var isAlreadyAt = Vector3.Distance(transform.position, position) < 0.1f;
        if (isAlreadyAt)
        {
            moveStatus.cantMoveReason = CantMoveReason.ALREADY_THERE;
            return moveStatus;
        }

        moveStatus.hinderPosition = null;
        moveStatus.cantMoveReason = CantMoveReason.CAN_MOVE;
        return moveStatus;
    }
    
    public GameObject GetFieldObject(Vector3 position)
    {
        var origin = position + Vector3.up * 5f;
        Debugger.DrawRay(origin, Vector3.down, checkDepth + 5, debugColor, .4f, false);
        return Physics.Raycast(origin, Vector3.down, out var hit, checkDepth + 5, groundLayerMask) ? hit.transform.gameObject : null;
    }

}
