using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public UnityEvent OnSwitchBetweenScreens;

    public void switchToSettingsScreen()
    {
        OnSwitchBetweenScreens?.Invoke();
    }
}
