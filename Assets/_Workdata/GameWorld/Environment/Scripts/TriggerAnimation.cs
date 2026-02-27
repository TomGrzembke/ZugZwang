using System;
using MyBox;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField] private string triggerName = "Open";
    [SerializeField, Tag] private string playerTag = "Player";
    [SerializeField] private Animator animToTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(playerTag)) return;

        animToTrigger.SetTrigger(triggerName);
    }
}
