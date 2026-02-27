using MyBox;
using UnityEngine;

public class EnvironmentDespawner : MonoBehaviour
{
    [SerializeField, Tag] private string tagToCheck;

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagToCheck)) return;

        var setups = other.transform.parent.GetComponentsInChildren<AssetSetup>();

        foreach (var setup in setups)
        {
            setup.Cleanup();
        }

        var transitionPacks = other.transform.parent.GetComponentsInChildren<TransitionPack>();

        foreach (var transitionPack in transitionPacks)
        {
            transitionPack.Cleanup();
        }
    }
}