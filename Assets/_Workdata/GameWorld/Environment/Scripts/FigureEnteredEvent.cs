using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class FigureEnteredEvent : MonoBehaviour
{
    [Separator("Trigger Settings")] [Space(10)] [SerializeField] [Tag]
    private string figureTag = "Player";

    [SerializeField] private bool disableOnCollision = true;

    [Separator("Events")] [Space(10)] [SerializeField]
    private UnityEvent onEnteredEvent;

    [SerializeField] private SegmentSetup segmentSetup;
    [SerializeField] private Collider coll;

    private void OnEnable()
    {
        segmentSetup.OnCleanup += Cleanup;
    }

    private void OnDisable()
    {
        segmentSetup.OnCleanup -= Cleanup;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(figureTag))
        {
            onEnteredEvent?.Invoke();
            if (disableOnCollision) coll.enabled = false;
        }
    }

    private void Cleanup()
    {
        coll.enabled = true;
    }
}