using MyBox;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    [Separator("References")]
    [SerializeField, Tooltip("Use the segment that can be swiped although the game is paused.")] private SwipableVFX swipableRow;

    [SerializeField, Tooltip("Use the highlight above the field.")]
    private GameObject plane;
    
    
    private void Awake()
    {
        swipableRow.OnFieldSwiped += ResetTimeScale;
    }

    private void Start()
    {
        Time.timeScale = 0;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1;
        swipableRow.OnFieldSwiped -= ResetTimeScale;
        plane.SetActive(false);
    }

    
}
