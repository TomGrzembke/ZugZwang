using MyBox;
using UnityEngine;
using System;
using System.Collections;

public class CoinCollectable : MonoBehaviour
{
    [SerializeField, Tag] private string playerTag = "Player";
    [SerializeField] private int scoreToAdd = 1;
    [SerializeField] private SegmentReferences segmentReferences;
    [SerializeField] private GameObject coinUIPrefab;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float animationDuration = 0.5f;

    public static event Action<Vector3> OnPickUp;

    public void SetSegmentRefSetup(SegmentReferences segmentRefs)
    {
        segmentReferences = segmentRefs;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Collect();
    }

    public void Collect()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider) boxCollider.enabled = false;

        segmentReferences.HighScoreObject.AddScore(scoreToAdd);

        DisableChildren();
        CollectCoin();

        OnPickUp?.Invoke(transform.position);
    }

    private void CollectCoin()
    {
        Canvas canvas = segmentReferences.CanvasReferences.GetComponent<Canvas>();
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            uiCamera,
            out Vector2 localStart
        );

        GameObject uiCoin = segmentReferences.ObjectPooling.SpawnObject(coinUIPrefab, canvas.transform.position, Quaternion.identity, ObjectPooling.PoolType.VFX);
        uiCoin.transform.SetParent(canvas.transform);
        uiCoin.transform.localScale = coinUIPrefab.transform.localScale;
        
        RectTransform coinRect = uiCoin.GetComponent<RectTransform>();
        coinRect.localPosition = localStart;

        Vector3 targetScreenPos =
            RectTransformUtility.WorldToScreenPoint(uiCamera, segmentReferences.CanvasReferences.scoreUI.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            targetScreenPos,
            uiCamera,
            out Vector2 localTarget
        );

        StartCoroutine(MoveCoin(coinRect, localTarget));
    }

    private IEnumerator MoveCoin(RectTransform coin, Vector2 targetLocalPos)
    {
        float time = 0;
        Vector3 start = coin.localPosition;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            coin.localPosition = Vector3.Lerp(start, targetLocalPos, animationCurve.Evaluate(t));
            yield return null;
        }

        segmentReferences.ObjectPooling.ReturnObjectToPool(coin.gameObject, ObjectPooling.PoolType.VFX);
    }

    private void DisableChildren()
    {
        foreach (Transform child in parentObject.transform)
        {
            if (!child.HasComponent<CoinCollectable>())
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}