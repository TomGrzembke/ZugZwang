using UnityEngine;

public class ActivateRandom : MonoBehaviour
{
    [SerializeField] private GameObject[] randoms;

    private void Start()
    {
        foreach (var random in randoms)
        {
            if (random != null && random.activeInHierarchy)
                return;
        }

        var activateObj = randoms[Random.Range(0, randoms.Length)];
        activateObj.SetActive(true);
    }
}