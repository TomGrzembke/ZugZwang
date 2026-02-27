using System;
using System.Collections;
using UnityEngine;

public class GolemAnimation : MonoBehaviour
{
    [SerializeField] private MovingRow movingRow;

    [SerializeField] private Animator anim;

    [SerializeField] private string animTrigger = "push";

    private RoundTimer roundTimer;


    private IEnumerator Start()
    {
        yield return null;
        roundTimer = movingRow.GetRoundtimer();
        roundTimer.OnEarlyTurn += Push;
    }

    private void OnEnable()
    {
        if (roundTimer != null)
            roundTimer.OnEarlyTurn += Push;
    }

    private void OnDisable()
    {
        if (roundTimer != null)
            roundTimer.OnEarlyTurn -= Push;
    }

    private void Push()
    {
        anim.SetTrigger(animTrigger);
    }
}