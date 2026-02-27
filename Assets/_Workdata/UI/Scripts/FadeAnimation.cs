using MyBox;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class FadeAnimation : MonoBehaviour
{
    [Separator("Animations")]
    [SerializeField] private bool fadeOut;
    public UnityEvent OnFadeComplete;

    public void StartFade()
    {
        Animator animator = GetComponent<Animator>();
        if (fadeOut)
            animator.SetTrigger("OnFadeOut");
        else
            animator.SetTrigger("OnFadeIn");
    }

    public void OnFadeFinished()
    {
        OnFadeComplete?.Invoke();
    }
}
