using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static List<object> CleanList(this List<object> list)
    {
        list.RemoveAll(x => x == null);

        return list;
    }

    public static List<GameObject> CleanList(this List<GameObject> list)
    {
        list.RemoveAll(x => x == null);

        return list;
    }

    public static List<GameObject> BubbleSortAfterX(this List<GameObject> objectsToCheck, bool posRising = false)
    {
        GameObject t;
        var a = objectsToCheck;

        for (var p = 0; p <= a.Count - 2; p++) // Outer loop for passes
        for (var i = 0; i <= a.Count - 2; i++) // Inner loop for comparison and swapping
        {
            if (posRising
                    ? a[i].transform.position.x <
                      a[i + 1].transform.position.x
                    : a[i].transform.position.x >
                      a[i + 1].transform.position.x) // Checking if the current element is smaller than the next element
            {
                t = a[i + 1]; // Swapping elements if they are in the wrong order
                a[i + 1] = a[i];
                a[i] = t;
            }
        }

        return a;
    }

    public static List<Transform> BubbleSortAfterX(this List<Transform> objectsToCheck)
    {
        Transform t;
        var a = objectsToCheck;

        for (var p = 0; p <= a.Count - 2; p++) // Outer loop for passes
        for (var i = 0; i <= a.Count - 2; i++) // Inner loop for comparison and swapping
            if (a[i].position.x <
                a[i + 1].position.x) // Checking if the current element is smaller than the next element
            {
                t = a[i + 1]; // Swapping elements if they are in the wrong order
                a[i + 1] = a[i];
                a[i] = t;
            }

        return a;
    }
}