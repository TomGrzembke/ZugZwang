using System.Collections.Generic;
using UnityEngine;

public class ListSorter
{
    public GameObject GetSecondLastXPosition(List<GameObject> objectsToCheck)
    {
        if (objectsToCheck.Count == 0) return null;
        var temp = objectsToCheck.BubbleSortAfterX(posRising: true);

        if (temp.Count == 1) return temp[temp.Count - 1];

        return temp[temp.Count - 2];
    }

    public GameObject GetLastXPosition(List<GameObject> objectsToCheck)
    {
        if (objectsToCheck.Count == 0) return null;

        var temp = objectsToCheck.BubbleSortAfterX(posRising: true);


        return temp[temp.Count - 1];
    }

    public GameObject GetMiddleXPosition(List<GameObject> objectsToCheck)
    {
        if (objectsToCheck.Count == 0) return null;

        var temp = objectsToCheck.BubbleSortAfterX(posRising: true);

        return temp[Mathf.FloorToInt(temp.Count / 2)];
    }

    public GameObject GetSecondXPosition(List<GameObject> objectsToCheck)
    {
        if (objectsToCheck.Count == 0) return null;
        var temp = objectsToCheck.BubbleSortAfterX(posRising: true);

        if (temp.Count == 1) return temp[0];

        return temp[1];
    }
    
}