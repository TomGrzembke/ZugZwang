using UnityEngine;

public class AnimationStartSettings : MonoBehaviour
{
    [SerializeField] private float animationTimeOffset;
    [SerializeField] private float animationScale = 1;
    [SerializeField] Animator anim;

    private void Start()
    {
        anim.speed = animationScale;
        anim.Play(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, animationTimeOffset);
    }
}