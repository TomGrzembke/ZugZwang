using System;
using System.Collections.Generic;
using MyBox;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

using UnityEngine;

[Serializable]
public class ObjectPrefabRelation
{
    public GameObject existingObject;
    public GameObject prefab;
}

public class AddExistingToPool : MonoBehaviour
#if UNITY_EDITOR
    , IPreprocessBuildWithReport
#endif
{
    [SerializeField] private List<ObjectPrefabRelation> objectsToPool;

    [SerializeField] private ObjectPooling.PoolType poolType = ObjectPooling.PoolType.GAMEOBJECTS;
    [SerializeField] private ObjectPooling objectPooling;
    [SerializeField] private bool deactivateOnStart;

    [SerializeField] private float editorAssetOffset = 50;


#if UNITY_EDITOR
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Starting scene modifications for pre-build...");

        var scene = EditorSceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            var prePoolers = root.GetComponentsInChildren<AddExistingToPool>();

            foreach (var prePooler in prePoolers)
            {
                prePooler.PrepPooling();
            }
        }
        
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("Finished scene modifications for pre-build...");
    }
#endif

    private void Awake()
    {
        PrepPooling();
    }

    public void PrepPooling()
    {
        if (!deactivateOnStart) return;

        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
    }

    private void Start()
    {
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            var objectToPool = objectsToPool[i].existingObject;

            objectPooling.AddExisting(objectToPool, objectsToPool[i].prefab, poolType);

            if (deactivateOnStart)
            {
                objectPooling.ReturnObjectToPool(objectToPool, poolType);
            }
        }
    }


#if UNITY_EDITOR
    [ButtonMethod]
    public void UpdateObjects()
    {
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            if (objectsToPool[i].existingObject != null) continue;

            objectsToPool[i].existingObject =
                PrefabUtility.InstantiatePrefab(objectsToPool[i].prefab, transform) as GameObject;
            var objectToPool = objectsToPool[i].existingObject;
            EditorUtility.SetDirty(objectToPool);

            objectToPool.SetActive(false);
        }

        EditorUtility.SetDirty(transform);
    }

    [ButtonMethod]
    public void RemoveUnused()
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            var shouldDelete = true;

            for (int j = 0; j < objectsToPool.Count; j++)
            {
                if (objectsToPool[j].existingObject == child)
                {
                    shouldDelete = false;
                    break;
                }
            }

            if (shouldDelete)
            {
                DestroyImmediate(child);
            }
        }

        EditorUtility.SetDirty(transform);
    }

    [ButtonMethod]
    public void ApplyOffset()
    {
        ApplyOffset(editorAssetOffset);
    }

    [ButtonMethod]
    public void ResetOffset()
    {
        ApplyOffset(0);
    }

    void ApplyOffset(float offset)
    {
        int childCount = transform.childCount;

        var currentOffset = offset;
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            child.transform.position = child.transform.position.ChangeZ(currentOffset);
            currentOffset += offset;
        }

        EditorUtility.SetDirty(transform);
    }
#endif
}