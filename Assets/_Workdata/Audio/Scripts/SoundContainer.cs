using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class SoundContainer : MonoBehaviour
{
    [SerializeField] private AutoMove autoMove;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource crashSound;
    [SerializeField] private AudioSource moveSound;
    [SerializeField] private GameObject figureMoveCollection;

    private List<AudioSource> moveSounds;

    private void Start()
    {
        SubscribeEvents();
        moveSounds = figureMoveCollection?.GetComponentsInChildren<AudioSource>().ToList();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        DesubscribeEvents();
    }

    private void OnDestroy()
    {
        DesubscribeEvents();
    }

    private void SubscribeEvents()
    {
        autoMove.OnDeath += PlayDeathSound;
        autoMove.OnFigureCrash += PlayCrashSound;
        autoMove.OnMoveStart += PlayMoveStartSound;
        autoMove.OnMoveEnd += PlayMoveEndSound;
    }

    private void DesubscribeEvents()
    {
        autoMove.OnDeath -= PlayDeathSound;
        autoMove.OnFigureCrash -= PlayCrashSound;
        autoMove.OnMoveStart -= PlayMoveStartSound;
        autoMove.OnMoveEnd -= PlayMoveEndSound;
    }
    
    private void PlayDeathSound()
    {
        if(!deathSound.isActiveAndEnabled) return;
        deathSound.Play();
    }

    private void PlayCrashSound()
    {
        if(!crashSound.isActiveAndEnabled) return;
        crashSound.Play();
    }

    private void PlayMoveStartSound()
    {
        if(!moveSound.isActiveAndEnabled) return;
        moveSound.Play();
    }
    
    private void PlayMoveEndSound()
    {
        var moveSound = moveSounds.GetRandom();
        
        if(!moveSound.isActiveAndEnabled) return;
        moveSound.Play();
    }
}