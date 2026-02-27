using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary> Original: https://youtu.be/Ah3epb2HGCw?si=SlNPhmcfvBHvwS1O , changed up for usecase and naming conv</summary>
public class ObjectPooling : MonoBehaviour
{
    private GameObject particleSystemsEmpty;
    private GameObject gameObjectsEmpty;
    private GameObject segmentsEmpty;
    private GameObject soundFXEmpty;

    private Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
    private Dictionary<GameObject /*Spawned*/, GameObject /*Prefab*/> cloneToPrefabMap;


    public enum PoolType
    {
        VFX = 10,
        GAMEOBJECTS = 20,
        SOUNDS = 30,
        SEGMENTS = 40,
    }

    private void Awake()
    {
        objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
        SetupEmpties();
    }


    private void SetupEmpties()
    {
        particleSystemsEmpty = new GameObject("VFX");
        particleSystemsEmpty.transform.SetParent(transform);

        gameObjectsEmpty = new GameObject("Game Objects");
        gameObjectsEmpty.transform.SetParent(transform);

        soundFXEmpty = new GameObject("Sounds");
        soundFXEmpty.transform.SetParent(transform);

        segmentsEmpty = new GameObject("Segments");
        segmentsEmpty.transform.SetParent(transform);
    }


    private void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot,
        PoolType poolType, GameObject existingObj = null)
    {
        ObjectPool<GameObject> pool =
            new ObjectPool<GameObject>(
                createFunc: () => CreateObjects(prefab, pos, rot, poolType, existingObj),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject
            );

        objectPools.Add(prefab, pool);
    }


    private GameObject CreateObjects(GameObject prefab, Vector3 pos, Quaternion rot,
        PoolType poolType, GameObject existingObj = null)
    {
        if (existingObj != null)
        {
            GameObject newParent = GetParentObject(poolType);
            existingObj.transform.SetParent(newParent.transform);
            return existingObj;
        }

        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = GetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);
        return obj;
    }

    private void OnGetObject(GameObject obj)
    {
    }

    private void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyObject(GameObject obj)
    {
        if (cloneToPrefabMap.ContainsKey(obj))
        {
            cloneToPrefabMap.Remove(obj);
        }
    }

    private GameObject GetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.VFX:
                return particleSystemsEmpty;

            case PoolType.GAMEOBJECTS:
                return gameObjectsEmpty;

            case PoolType.SOUNDS:
                return soundFXEmpty;
            case PoolType.SEGMENTS:
                return segmentsEmpty;
            default:
                return null;
        }
    }

    private T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation,
        PoolType poolType) where T : Object
    {
        if (!objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = objectPools[objectToSpawn].Get();

        if (obj.activeInHierarchy)
        {
            obj = CreateObjects(objectToSpawn, spawnPos, spawnRotation, poolType);
        }


        if (obj != null)
        {
            if (!cloneToPrefabMap.ContainsKey(obj))
            {
                cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }

    public T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation,
        PoolType poolType) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation,
        PoolType poolType)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }


    public void AddExisting(GameObject obj, GameObject prefab, PoolType poolType)
    {
        if (obj == null) return;
        if (prefab == null) return;

        if (!objectPools.ContainsKey(prefab))
        {
            CreatePool(prefab, obj.transform.position, Quaternion.identity, poolType, obj);
        }

        if (!cloneToPrefabMap.ContainsKey(obj))
        {
            cloneToPrefabMap.Add(obj, prefab);
        }

        GameObject newParent = GetParentObject(poolType);
        obj.transform.SetParent(newParent.transform);

        obj.SetActive(true);
    }

    public bool ReturnObjectToPool(GameObject obj, PoolType poolType)
    {
        if (cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = GetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                if (!obj.activeInHierarchy) return false;
                obj.transform.SetParent(parentObject.transform);
            }

            if (objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                if (!obj.activeInHierarchy) return false;

                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);

                obj.SetActive(false);
            }

            obj.SetActive(false);
            return true;
        }
        else
        {
            Debug.LogWarning("Disabled an Object that was not pooled: " + obj.name);

            return false;
        }
    }
}