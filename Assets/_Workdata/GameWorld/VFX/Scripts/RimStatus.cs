using System.Collections.Generic;
using UnityEngine;

public class RimStatus : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> meshRendererToUpdate;
    [SerializeField] private AutoMove autoMove;
    [SerializeField] private Color lavaIntentColor = Color.red;
    [SerializeField] private GameObject flameVfx;


    private void OnEnable()
    {
        autoMove.OnLavaIntent += ChangeToLava;
        autoMove.OnIntent += ResetColors;
    }

    private void OnDisable()
    {
        autoMove.OnLavaIntent -= ChangeToLava;
        autoMove.OnIntent -= ResetColors;
    }

    private void ChangeToLava(Vector3 _)
    {
        foreach (var renderer in meshRendererToUpdate)
        {
            if (!renderer.material.HasProperty("_MainTint") && !renderer.material.HasProperty("_VisualEffects")) continue;

            renderer.material.SetColor("_MainTint", lavaIntentColor);
            renderer.material.SetInt("_VisualEffects", 1);
        }

        if (flameVfx != null)
        {
            flameVfx.SetActive(true);
        }
    }

    private void ResetColors(Vector3 _)
    {
        var currentStatus = autoMove.fieldStatusChecker.GetFieldStatus(autoMove.transform.position);

        if (currentStatus.cantMoveReason == CantMoveReason.LAVA_FIELD)
        {
            return;
        }
        
        foreach (var renderer in meshRendererToUpdate)
        {
            if (!renderer.material.HasProperty("_VisualEffects")) continue;
            renderer.material.SetInt("_VisualEffects", 0);
        }
        
        if (flameVfx != null)
        {
            flameVfx.SetActive(false);
        }
    }
}
