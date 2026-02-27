using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceChangeMonitor : MonoBehaviour
{
    void OnEnable()
    {
        InputSystem.onDeviceChange += OnInputDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnInputDeviceChange;
    }

    void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"Input Device Change: Device = {device.name}, Change = {change}");
        // You can add more specific logging here to track device IDs, etc.
    }
}