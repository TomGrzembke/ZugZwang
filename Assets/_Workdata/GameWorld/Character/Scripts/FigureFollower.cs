using System.Collections.Generic;
using UnityEngine;

enum FollowMode
{
    NONE = 0,
    LAST = 10,
    SECONDLAST = 20,
    MIDDLE = 30,
    SECOND = 40,
}

public class FigureFollower : MonoBehaviour
{
    private ListSorter listSorter = new ListSorter();
    [SerializeField] private FollowMode followMode = FollowMode.SECONDLAST;

    [SerializeField] private FigureFactory figureFactory;
    [SerializeField] private FollowCoordinates camTargeter;

    private void Awake()
    {
        figureFactory.OnFigureSpawnedListChanged += UpdateTarget;
    }

    private void UpdateTarget(List<GameObject> objectsToCheck)
    {
        GameObject target = null;

        switch (followMode)
        {
            case FollowMode.LAST:
                target = listSorter.GetLastXPosition(objectsToCheck);
                break;
            case FollowMode.SECONDLAST:
                target = listSorter.GetSecondLastXPosition(objectsToCheck);
                break;
            case FollowMode.MIDDLE:
                target = listSorter.GetMiddleXPosition(objectsToCheck);
                break;
            case FollowMode.SECOND:
                target = listSorter.GetSecondXPosition(objectsToCheck);
                break;
            default:
                break;
        }

        if (target == null)
        {
            Debug.Log("No followTarget found", this);
            return;
        }

        camTargeter.SetTarget(target.transform);
    }

    private void OnDestroy()
    {
        figureFactory.OnFigureSpawnedListChanged -= UpdateTarget;
    }
}