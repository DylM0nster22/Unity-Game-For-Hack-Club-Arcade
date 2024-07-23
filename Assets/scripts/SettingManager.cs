using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] [Tooltip("This allows for pausing of the game but not needed in the main menu")] bool _isMainMenu;
    [SerializeField] Animator _anim;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("UI Elements")]
    public GameObject settingsPanel;
    public Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider brightnessSlider;
    public Image brightnessOverlay;
    public Slider fpsLimitSlider;
    public Toggle fpsCounterToggle;
    public Slider mouseSensitivitySlider;
    public Button applyButton;
    public Button cancelButton;

    [Header("TextBoxes")]
    [SerializeField] TextMeshProUGUI _normalHighScore_Text;
    [SerializeField] TextMeshProUGUI _pacifistHighScore_Text;
    [SerializeField] TextMeshProUGUI _musicVolume_Text;
    [SerializeField] TextMeshProUGUI _soundEffectsVolume_Text;

    [Header("Scripts")]
    public FPSCounter fpsCounter;
    public PauseScreenUI pauseScreenUI;

    private Resolution[] resolutions;
    private bool _isSettingsMenuActive = false;

    void Start()
    {
        InitializeUI();
        LoadSettings();
    }

    void InitializeUI()
    {
        // Setup quality dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        // Setup resolution dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Setup fullscreen toggle
        fullscreenToggle.isOn = Screen.fullScreen;

        // Setup volume sliders
        SetupVolumeSlider(masterVolumeSlider, "MasterVolume", _musicVolume_Text);
        SetupVolumeSlider(musicVolumeSlider, "MusicVolume", _musicVolume_Text);
        SetupVolumeSlider(sfxVolumeSlider, "SFXVolume", _soundEffectsVolume_Text);

        // Setup brightness slider
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        SetBrightness(brightnessSlider.value);

        // Setup FPS Limit Slider
        fpsLimitSlider.onValueChanged.AddListener(SetFPSLimit);

        // Setup FPS Counter Toggle
        fpsCounterToggle.onValueChanged.AddListener(ToggleFPSCounter);

        // Setup Mouse Sensitivity Slider
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        // Setup button listeners
        applyButton.onClick.AddListener(ApplySettings);
        cancelButton.onClick.AddListener(CloseSettings);

        PullIntFromPlayerPrefs("HighScore", _normalHighScore_Text, true);
        PullIntFromPlayerPrefs("PacifistHighScore", _pacifistHighScore_Text, true);
    }

    public void LoadSettings()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel", 3));
        Screen.SetResolution(PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width),
                             PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height),
                             Screen.fullScreen);
        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        SetVolume("MasterVolume", masterVolumeSlider.value);
        SetVolume("MusicVolume", musicVolumeSlider.value);
        SetVolume("SFXVolume", sfxVolumeSlider.value);

        SetBrightness(brightnessSlider.value);

        fpsLimitSlider.value = PlayerPrefs.GetInt("FPSLimit", 60);
        fpsCounterToggle.isOn = PlayerPrefs.GetInt("FPSCounterEnabled", 1) == 1;
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    private void SetupVolumeSlider(Slider slider, string key, TextMeshProUGUI text)
    {
        slider.value = PlayerPrefs.GetFloat(key, 1f);
        UpdateVolumeText(slider.value, text);
        slider.onValueChanged.AddListener((value) => {
            SetVolume(key, value);
            UpdateVolumeText(value, text);
        });
    }

    private void UpdateVolumeText(float value, TextMeshProUGUI text)
    {
        text.text = Mathf.RoundToInt(value * 100).ToString();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(string parameter, float value)
    {
        audioMixer.SetFloat(parameter, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(parameter, value);
    }

    public void SetBrightness(float brightness)
    {
        float alpha = 1f - brightness;
        Color overlayColor = brightnessOverlay.color;
        overlayColor.a = alpha;
        brightnessOverlay.color = overlayColor;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }

    void SetFPSLimit(float limit)
    {
        Application.targetFrameRate = Mathf.RoundToInt(limit);
    }

    void ToggleFPSCounter(bool enabled)
    {
        if (fpsCounter != null)
        {
            fpsCounter.ShowFPSCounter(enabled);
        }
    }

    void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    void ApplySettings()
    {
        PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionWidth", resolutions[resolutionDropdown.value].width);
        PlayerPrefs.SetInt("ResolutionHeight", resolutions[resolutionDropdown.value].height);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FPSLimit", Mathf.RoundToInt(fpsLimitSlider.value));
        PlayerPrefs.SetInt("FPSCounterEnabled", fpsCounterToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        // EventManager.UpdateMusicVolume?.Invoke();
        // EventManager.UpdateSoundEffectVolume?.Invoke();
    }

    public void ToggleSettingsMenu()
    {
        _isSettingsMenuActive = !_isSettingsMenuActive;
        settingsPanel.SetActive(_isSettingsMenuActive);

        if (!_isMainMenu)
        {
            Time.timeScale = _isSettingsMenuActive ? 0 : 1;
        }

        _anim.SetTrigger("Toggle");
    }

    public void OpenSettings()
    {
        ToggleSettingsMenu();
    }

    void CloseSettings()
    {
        LoadSettings(); // Revert to previous settings
        ToggleSettingsMenu();
        if (pauseScreenUI != null)
        {
            pauseScreenUI.ShowPauseMenu(true);
        }
    }

    public void ResetNormalHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        _normalHighScore_Text.text = "0";
    }

    public void ResetPacifistHighScore()
    {
        PlayerPrefs.SetInt("PacifistHighScore", 0);
        _pacifistHighScore_Text.text = "0";
    }

    private int PullIntFromPlayerPrefs(string keyName, TextMeshProUGUI textBox, bool isScore)
    {
        int num = isScore ? 0 : 100;
        if (PlayerPrefs.HasKey(keyName))
            num = PlayerPrefs.GetInt(keyName);
        textBox.text = num.ToString();
        return num;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}