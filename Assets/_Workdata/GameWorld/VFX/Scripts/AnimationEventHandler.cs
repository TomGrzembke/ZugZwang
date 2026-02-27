using UnityEngine;
using UnityEngine.Events;

/// <summary> This script should be attached to an animated 
///Object to utilize the Unity Events through this </summary>
public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] UnityEvent[] onAnimEvents;

    /// <param name="eventID"> The ID of the corresponding onEvent array ID</param>
    void OnAnimEventID(int eventID)
    {
        onAnimEvents[eventID]?.Invoke();
    }

    /// <summary> Be careful with statemachine 
    ///blending and instantiating! Might need a spawn limit per frame bunch</summary>
    void InstantiateObj(Object obj)
    {
        Instantiate(obj, transform.localPosition, Quaternion.identity);
    }
}
