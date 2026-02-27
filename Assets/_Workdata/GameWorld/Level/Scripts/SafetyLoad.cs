using MyBox;
using UnityEngine;

public class SafetyLoad : MonoBehaviour
{
    [SerializeField] private FieldStatusChecker fieldStatusChecker;
    [SerializeField] private NextSegmentRequester nextSegmentRequester;

    [SerializeField, Tag] private string playerTag;
    [SerializeField] private float checkStart = 5;
    [SerializeField] FieldScaleSO fieldScaleSO;

    [SerializeField] private float rowsToCheck = 5;
    private bool wasUsed;

    private void OnTriggerEnter(Collider other)
    {
        if(wasUsed) return;
        if (!other.CompareTag(playerTag)) return;

        wasUsed = true;
        
        var checkStartPos = transform.position.AddX(checkStart);
        var hasObject = false;

        for (int i = 0; i < rowsToCheck; i++)
        {
            var obj = fieldStatusChecker.GetFieldObject(checkStartPos.AddX(i * fieldScaleSO.fieldMultiplier.x));
            if (obj != null)
            {
                hasObject = true;
                break;
            }
        }

        if (!hasObject)
        {
            nextSegmentRequester.SafetyLoadNext();
        }
    }
}