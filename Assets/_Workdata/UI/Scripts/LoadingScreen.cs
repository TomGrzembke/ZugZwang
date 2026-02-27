using MyBox;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class LoadingScreen : MonoBehaviour
{
    [Separator("References")]
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentText;
    [Space(20)]
    [Separator("Texts")]
    [SerializeField, TextArea] private string[] loadingTexts;
    private SceneLoader loader;

    private void Awake()
    {
        loader = GetComponent<SceneLoader>();
    }

    private void OnEnable()
    {
        loader.OnSceneLoading += UpdateProgressBar;
    }
    
    private void OnDestroy()
    {
        loader.OnSceneLoading -= UpdateProgressBar;
    }

    private void OnDisable()
    {
        loader.OnSceneLoading -= UpdateProgressBar;
    }

    private void Start()
    {
        progressBarFill.fillAmount = 0f;
        loadingText.text = loadingTexts.GetRandom();
    }

    private void UpdateProgressBar(float progress)
    {
        progressBarFill.fillAmount = progress;
        percentText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    public void StartLoading()
    {
        loader.LoadSceneByNameAsync();
    }
}
