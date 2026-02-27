using MyBox;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsOverwriter : MonoBehaviour
{
    [Separator("Graphic Settings")]
    [SerializeField] private int defaultGraphicIndex = 2;
    [SerializeField] private GraphicsSO graphicsSO;
    [Separator("Sound Settings")]
    [SerializeField] private AudioMixerGroup soundGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    private void Start()
    {
        int graphicIndex;
        if(PlayerPrefs.HasKey("graphicsIndex")) graphicIndex = Prefs.GraphicIndex;
        else
        {
            graphicIndex = defaultGraphicIndex;
            Prefs.GraphicIndex = defaultGraphicIndex;
        }
        
        SetGraphicSettings(graphicIndex);
        SetSoundSettings();
    }

    public void SetGraphicSettings(int currentGraphicIndex)
    {
        graphicsSO.SetGraphicSettings(currentGraphicIndex);
    }

    private void SetSoundSettings()
    {
        // DB -> Linear: Mathf.Pow(10, db / 20f)
        // Linear -> DB: Mathf.Log10(value) * 20
        AudioMixer audioMixer = soundGroup.audioMixer ? soundGroup.audioMixer : musicGroup.audioMixer;

        if (audioMixer == null) return;
        
        float defaultDB = -6f;
        float musicDB = PlayerPrefs.HasKey("musicVolume") ? Prefs.MusicVolume : defaultDB;
        float soundDB = PlayerPrefs.HasKey("soundVolume") ? Prefs.SoundVolume : defaultDB;
        
        audioMixer.SetFloat("Music", musicDB);
        audioMixer.SetFloat("SFX", soundDB);
        
    }
}
