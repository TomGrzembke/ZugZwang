using UnityEngine;

public class RandomTextureOffset : MonoBehaviour // Yona
{
    [SerializeField] private float offsetMaximum = 3;
    private const string SHADER_PROPERTY = "_OffsetTexture";
    private Material material;
    private float offset;

    private void OnEnable()
    {
        material = GetComponent<MeshRenderer>().material;
        offset = Random.Range(0,3f);
        
        material.SetFloat(SHADER_PROPERTY, offset);

    }
}
