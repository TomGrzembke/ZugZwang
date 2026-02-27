using MyBox;
using UnityEngine;

public class FieldLooping : MonoBehaviour
{
    [Separator("General Settings")] [SerializeField] [Tag]
    private string tagToCompare = "Field";
    [SerializeField] [Tag] private string lavaFieldTag = "LavaField";
    
    [SerializeField] private int segmentThickness = 5;

    [Space(10)] [Separator("Down Movement")] [SerializeField]
    private float downDepth = 2.5f;

    [SerializeField] private float downTime = .3f;
    [SerializeField] private AnimationCurve downCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Space(10)] [Separator("Side Movement")] [SerializeField]
    private float sideTime = .1f;

    [SerializeField] private AnimationCurve sideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Space(10)] [Separator("Up Movement")] [SerializeField]
    private float upTime = .3f;

    [SerializeField] private AnimationCurve upCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagToCompare) && !other.CompareTag(lavaFieldTag)) return;

        var otherTrans = other.transform;
        var side = otherTrans.position.z - transform.position.z < 0 ? 1 : -1;

        var move = other.GetComponent<MoveObject>();

        move.LoopOver(side, downDepth, downTime, downCurve, upTime, upCurve, sideTime, segmentThickness, sideCurve);
    }
}