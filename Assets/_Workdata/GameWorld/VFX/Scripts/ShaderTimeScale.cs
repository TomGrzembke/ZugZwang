using UnityEngine;
using UnityEditor; 

[ExecuteInEditMode]
public class ShaderTimeScale : MonoBehaviour
{

    private void OnEnable()
    {

        if (!Application.isPlaying)
        {
            Shader.SetGlobalFloat("_UnscaledTime", Time.time);
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            Shader.SetGlobalFloat("_UnscaledTime", Time.time);
        }

        else
        {
            Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
        }
        
        
    }
}