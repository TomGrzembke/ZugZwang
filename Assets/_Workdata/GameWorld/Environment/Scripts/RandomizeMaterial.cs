using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] private List<Material> possibleLitMaterials;
    [SerializeField] private bool randomizeOrientation;

    private void Awake()
    {
        Randomize();
    }

    public void Randomize()
    {
        Material randomMat;

        randomMat = possibleLitMaterials[Random.Range(0, possibleLitMaterials.Count)];

        GetComponent<MeshRenderer>().material = randomMat;

        if (!randomizeOrientation) return;
        var rand1 = Random.Range(0, 2);
        var rand2 = Random.Range(0, 2);

        transform.localScale = new Vector3(rand1 == 0 ? transform.localScale.x : -transform.localScale.x,
            transform.localScale.y, rand2 == 0 ? transform.localScale.z : -transform.localScale.z);
    }
}