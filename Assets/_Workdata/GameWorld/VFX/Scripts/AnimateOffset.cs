using UnityEngine;

/// <summary> Utilizes world position and modifier to simulate an animation offset </summary>
public class AnimateOffset : MonoBehaviour
{
    [SerializeField] float delayPerDistance = .02f;
    
    [SerializeField] Animator anim;
    
    private void Awake()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        float offset = (transform.position.x + transform.position.z) * delayPerDistance;

        
        anim.Play(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, offset);
    }
}