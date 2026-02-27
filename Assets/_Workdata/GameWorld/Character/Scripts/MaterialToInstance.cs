using UnityEngine;

public class MaterialToInstance : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        var material = meshRenderer.material;

        Material newMat = material;

        meshRenderer.material = newMat;
    }
}
