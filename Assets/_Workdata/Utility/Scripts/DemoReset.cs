using System.Collections;
using UnityEngine;

public class DemoReset : MonoBehaviour
{
    [SerializeField] private BuildSettingsSO buildSettingsSO;
    [SerializeField] private float resetTime = 12;
    [SerializeField] private float resetTextStayTime = 3;
    [SerializeField] private GameObject resetText;

    [SerializeField] private SceneLoader sceneLoader;

    private const string MAIN_MENU_NAME = "MainMenu";

    private IEnumerator Start()
    {
        if (!buildSettingsSO.ExhibitVersion) yield break;
        //if(PlayerPrefs.GetInt("firstGame") == 1) yield break;

        yield return new WaitForSecondsRealtime(resetTime);

        PlayerPrefs.SetInt("firstGame", 1);

        if (gameObject.scene.name != MAIN_MENU_NAME)
        {
            sceneLoader.SetTargetSceneAndLoad(MAIN_MENU_NAME);
        }

        ResetFeedback(true);

        yield return new WaitForSecondsRealtime(resetTextStayTime);

        ResetFeedback(false);
    }

    private void ResetFeedback(bool condition)
    {
        if (resetText == null) return;

        resetText.SetActive(condition);
    }
}