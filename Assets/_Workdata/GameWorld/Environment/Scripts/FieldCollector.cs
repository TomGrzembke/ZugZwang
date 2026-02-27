using MyBox;
using UnityEditor;
using UnityEngine;

/// <summary> Debug tool to fasten up move row collection of available fields for pushing them </summary>
public class FieldCollector : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SegmentSetup segmentSetup;
    [SerializeField] private SegmentReferences segmentReferences;


    [ButtonMethod]
    public void CollectFields()
    {
        var moveRows = transform.GetComponentsInChildren<MoveRow>();
        Vector2 beforeAfterValues = new();

        foreach (var moveRow in moveRows) beforeAfterValues += moveRow.CollectFields();

        Debug.Log($" {beforeAfterValues.x} Fields Cleared and {beforeAfterValues.y} added!");
    }

    [ButtonMethod]
    public void ReapplyBendStrength()
    {
        var readers = transform.GetComponentsInChildren<SubshaderReader>(true);

        foreach (var reader in readers) reader.ApplyValue();
    }


    [ButtonMethod]
    public void SetupMovingRows()
    {
        var movingRows = transform.GetComponentsInChildren<MovingRow>(true);
        int count = 0;

        foreach (var movingRow in movingRows)
        {
            movingRow.SetSegmentReferenceSetup(segmentReferences);
            EditorUtility.SetDirty(movingRow);
            count++;
        }

        Debug.Log($" {count} Moving Rows added!");
    }


    [ButtonMethod]
    public void SetupSwipeableVFX()
    {
        var swipeables = transform.GetComponentsInChildren<SwipableVFX>(true);
        int count = 0;

        foreach (var swipeable in swipeables)
        {
            swipeable.SetSegmentRefSetup(segmentReferences);
            EditorUtility.SetDirty(swipeable);
            count++;
        }

        Debug.Log($" {count} SwipeableVFX added!");
    }

    [ButtonMethod]
    public void SetupCoins()
    {
        var coins = transform.GetComponentsInChildren<CoinCollectable>(true);
        int count = 0;

        foreach (var coin in coins)
        {
            coin.SetSegmentRefSetup(segmentReferences);
            EditorUtility.SetDirty(coin);
            count++;
        }

        Debug.Log($" {count} Coins added!");
    }

    [ButtonMethod]
    ///Importnat setup for pooling access
    public void SetupAssetSetups()
    {
        var assetSetups = segmentSetup.GetComponentsInChildren<AssetSetup>(true);
        int count = 0;

        foreach (var assetSetup in assetSetups)
        {
            assetSetup.SetSegmentSetup(segmentSetup);
            EditorUtility.SetDirty(assetSetup);
            count++;
        }

        Debug.Log($" {count} AssetSetups Updated!");

        var transitionSetups = segmentSetup.GetComponentsInChildren<TransitionPack>(true);
        count = 0;

        foreach (var assetSetup in transitionSetups)
        {
            assetSetup.SetSegmentSetup(segmentSetup);
            EditorUtility.SetDirty(assetSetup);
            count++;
        }

        Debug.Log($" {count} TransitionSetups Updated!");
    }

    [ButtonMethod]
    public void DoAllActions()
    {
        CollectFields();
        ReapplyBendStrength();
        SetupMovingRows();
        SetupSwipeableVFX();
        SetupCoins();
        SetupAssetSetups();
    }
#endif
}