using UnityEngine;

public interface ISwipable
{
    void OnSwiped(Vector2 swipeDir);

    void OnSelect(Ray hit);
    void OnDeselect(Ray hit);
}