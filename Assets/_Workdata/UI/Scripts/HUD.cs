using MyBox;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Separator("UI References")]
    private TextMeshProUGUI text;
    private GameObject pauseButton;
    [SerializeField] private GameObject roundTimer;

    [Separator("Aspect Ratio Settings")] [SerializeField]
    private GameObject mobileHUD;
    [SerializeField] private GameObject desktopHUD;
    
    private GameObject hudObject;
    private GameObject pauseMenu;
    private SimpleTimer timer;


    private void Awake()
    {
        timer = GetComponentInChildren<SimpleTimer>();
        pauseMenu = GetComponentInChildren<PauseMenu>(true).gameObject;
        
        if (timer) 
            timer.OnTimeLeft += UpdateTime;
    }

    private void Start()
    {
        desktopHUD.SetActive(false);
        mobileHUD.SetActive(false);
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            hudObject = desktopHUD;
            desktopHUD.SetActive(true);
            mobileHUD.SetActive(false);
        }
        else
        {
            hudObject = mobileHUD;
            mobileHUD.SetActive(true);
            desktopHUD.SetActive(false);
        }

        text = hudObject.GetComponentInChildren<TextMeshMarkable>(true).GetComponent<TextMeshProUGUI>();
        pauseButton = hudObject.GetComponentInChildren<ButtonMarkable>(true).gameObject;
    }

    private void OnDestroy()
    {
        if (timer) 
            timer.OnTimeLeft -= UpdateTime;
    }

    private void SwitchFigureMovement()
    {
        roundTimer.SetActive(!roundTimer.activeSelf);
    }

    public void OnPauseMenuClicked()
    {
        SwitchPauseMenuVisibility();
        SwitchFigureMovement();
    }

    public void OnQuitButtonClicked()
    {
        text.gameObject.SetActive(true);
        SwitchPauseMenuVisibility();
        timer.StartTimer();
    }

    public void ResumeGame()
    {
        text.gameObject.SetActive(false);
        pauseButton.SetActive(true);
        
        SwitchFigureMovement();
    }

    public void UpdateTime(float time)
    {
        text.text = Mathf.Ceil(time).ToString();
    }

    private void SwitchPauseMenuVisibility()
    {
        hudObject.SetActive(!hudObject.activeInHierarchy);
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
}