using MyBox;
using UnityEngine;

public class FollowCoordinates : MonoBehaviour
{
    [Separator("Follow Settings")] [SerializeField]
    private Vector3 whichAxisToFollow = Vector3.one;

    [SerializeField] private Transform target;
    private Vector3 startPos;

    [SerializeField] private AnimationCurve smoothCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float smoothModifier = 1;
    [SerializeField] private Vector3 followOffset;
    
    private void Start()
    {
        startPos = transform.position;
        transform.position = startPos.Add(followOffset);
        ApplyPosition();
        
    }

    private void FixedUpdate()
    {
        ApplyPosition();
    }

    private void ApplyPosition()
    {
        if (target == null) return;
        var x = Mathf.Lerp(startPos.x, target.position.x, whichAxisToFollow.x);
        var y = Mathf.Lerp(startPos.y, target.position.y, whichAxisToFollow.y);
        var z = Mathf.Lerp(startPos.z, target.position.z, whichAxisToFollow.z);

        var pos = Vector3.Lerp(transform.position, new Vector3(x, y, z).Add(followOffset), smoothCurve.Evaluate(smoothModifier));
        
        transform.localPosition = pos;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}