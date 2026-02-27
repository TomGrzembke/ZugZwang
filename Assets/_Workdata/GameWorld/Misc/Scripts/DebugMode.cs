using System;
using MyBox;
using TMPro;
using UnityEngine;

[Serializable]
public class DebugMode : MonoBehaviour
{
    [Separator("Debug Mode")] [SerializeField]
    private bool debugMode;

    [Separator("Individual Settings")] [ConditionalField(nameof(debugMode))] [Range(0f, 2f)] [SerializeField]
    private float gameSpeed = 1f;

    [ConditionalField(nameof(debugMode))] [SerializeField]
    private bool showDebugTexts = true;

#pragma warning disable CS0414
    [ConditionalField(nameof(debugMode), nameof(showDebugTexts))] [SerializeField] [ReadOnly]
    
    private string Information = "Only texts with 'DEBUG_' count.";
#pragma warning restore CS0414

    [ConditionalField(nameof(debugMode))] [SerializeField]
    private bool limitFPS = true;

    [ConditionalField(nameof(limitFPS), nameof(debugMode))] [SerializeField]
    private int maxFPS = 60;

    [Separator("Additional Information")] [SerializeField] [ReadOnly]
    private string companyName;

    [SerializeField] [ReadOnly] private string productName;
    [SerializeField] [ReadOnly] private string unityVersion;
    [SerializeField] [ReadOnly] private string version;
    [SerializeField] [ReadOnly] private string identifier;
    [SerializeField] [ReadOnly] private string backgroundLoadingPriority;
    [SerializeField] [ReadOnly] private string platform;
    [SerializeField] [ReadOnly] private string targetFrameRate;
    [SerializeField] [ReadOnly] private string dataPath;
    [SerializeField] [ReadOnly] private string persistentDataPath;
    [SerializeField] [ReadOnly] private string consoleLogPath;
    [SerializeField] [ReadOnly] private string streamingAssetsPath;
    [SerializeField] [ReadOnly] private string temporaryCachePath;

    private void Start()
    {
        if ((debugMode && !showDebugTexts) || !debugMode)
        {
            var textFields = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            foreach (var textField in textFields)
                if (textField.name.StartsWith("DEBUG_"))
                    textField.gameObject.SetActive(false);
        }

        if (debugMode && limitFPS) Application.targetFrameRate = maxFPS;

        if (debugMode) Time.timeScale = gameSpeed;
    }

    public void UpdateEditorValues()
    {
        companyName = Application.companyName;
        productName = Application.productName;
        identifier = Application.identifier;
        version = Application.version;
        unityVersion = Application.unityVersion;
        platform = Application.platform.ToString();
        backgroundLoadingPriority = Application.backgroundLoadingPriority.ToString();
        temporaryCachePath = Application.temporaryCachePath;
        persistentDataPath = Application.persistentDataPath;
        consoleLogPath = Application.consoleLogPath;
        dataPath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;
        targetFrameRate = Application.targetFrameRate.ToString();
    }
}