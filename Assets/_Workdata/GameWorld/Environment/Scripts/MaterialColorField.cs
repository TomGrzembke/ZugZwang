using System.Collections;
using UnityEngine;

public class MaterialColorField : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MoveObject moveObject;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private FieldScaleSO fieldScaleSO;
    [SerializeField] private AnimationCurve smoothCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private bool onlyAtStart;
    private Coroutine colorBlendCoroutine;

    private Color initColor;
    private Material initMat;
    private Material instancedMat;

    /// <summary> inverts even rows to get a checker texture </summary>
    private bool invertColor;

    private void Awake()
    {
        if (onlyAtStart)
        {
            StartUpdateColor();
            return;
        }

        moveObject.OnMove += StartUpdateColor;
        moveObject.OnMoveFinished += EndUpdateColor;
    }

    private void Start()
    {
        invertColor = transform.position.x / fieldScaleSO.fieldMultiplier.x % 2 == 0;
        CreateMaterial();
        UpdateColor();
    }

    private void OnDestroy()
    {
        if (onlyAtStart)
        {
            return;
        }

        moveObject.OnMove -= StartUpdateColor;
        moveObject.OnMoveFinished -= EndUpdateColor;
    }

    private void CreateMaterial()
    {
        initMat = meshRenderer.material;
        initColor = initMat.GetColor("_Top_Color");
        instancedMat = new Material(initMat);
        instancedMat.SetColor("_Top_Color", secondaryColor);
        meshRenderer.material = instancedMat;
    }

    private void StartUpdateColor(int _ = 0)
    {
        EndUpdateColor();

        if (!isActiveAndEnabled) return;

        colorBlendCoroutine = StartCoroutine(LiveColorBlendCor());
    }

    private void EndUpdateColor(int _ = 0)
    {
        if (colorBlendCoroutine == null) return;

        StopCoroutine(colorBlendCoroutine);
    }

    private IEnumerator LiveColorBlendCor()
    {
        while (true)
        {
            UpdateColor();
            yield return null;
        }
    }

    private void UpdateColor()
    {
        //0 or 1 when object is on the scaling grid, gives between values if not
        var currentMoveAlpha = Mathf.Abs(transform.position.z / fieldScaleSO.fieldMultiplier.z % 2);

        //Transform values above 1 and invert from ex. 0.1 to 0.9
        if (currentMoveAlpha > 1) currentMoveAlpha = 2 - currentMoveAlpha;

        if (invertColor) currentMoveAlpha = 1 - currentMoveAlpha;

        instancedMat.SetColor("_Top_Color",
            Color.Lerp(initColor, secondaryColor, smoothCurve.Evaluate(currentMoveAlpha)));
    }
}