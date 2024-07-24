using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider brightnessSlider;
    public Slider audioSlider;
    public Button saveButton;
    public Button backButton;

    private GameObject settingsCanvas;

    private void Start()
    {
        // Create and initialize sliders and buttons
        CreateSettingsUI();

        // Initialize sliders with saved values or defaults
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        audioSlider.value = PlayerPrefs.GetFloat("Audio", 1f);

        // Add listeners to handle value changes
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        audioSlider.onValueChanged.AddListener(SetAudio);

        // Add listeners to buttons
        saveButton.onClick.AddListener(SaveAndClose);
        backButton.onClick.AddListener(CloseSettingsMenu);

        // Initially hide the settings menu
        settingsCanvas.SetActive(false);

        // Apply initial settings
        SetBrightness(brightnessSlider.value);
        SetAudio(audioSlider.value);
    }

    private void CreateSettingsUI()
    {
        // Create a Canvas for the settings menu
        settingsCanvas = new GameObject("SettingsCanvas");
        settingsCanvas.transform.SetParent(this.transform, false);
        Canvas canvas = settingsCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        settingsCanvas.AddComponent<CanvasScaler>();
        settingsCanvas.AddComponent<GraphicRaycaster>();

        // Create Background
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(settingsCanvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.6f, 1f); // Light blue color
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;

        // Create Brightness Slider
        brightnessSlider = CreateSlider("BrightnessSlider", settingsCanvas.transform, new Vector2(0.5f, 0.6f), "Brightness");

        // Create Audio Slider
        audioSlider = CreateSlider("AudioSlider", settingsCanvas.transform, new Vector2(0.5f, 0.4f), "Audio");

        // Create Save Button
        saveButton = CreateButton("SaveButton", "Save", new Vector2(0.5f, 0.2f));

        // Create Back Button
        backButton = CreateButton("BackButton", "Back", new Vector2(0.5f, 0.1f));
    }

    private Slider CreateSlider(string name, Transform parent, Vector2 anchorPosition, string labelText)
    {
        // Create Slider GameObject
        GameObject sliderObject = new GameObject(name);
        sliderObject.transform.SetParent(parent, false);

        // Add Slider component
        Slider slider = sliderObject.AddComponent<Slider>();

        // Configure Slider
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Create Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObject.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = Color.gray;
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;

        // Create Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.sizeDelta = Vector2.zero;

        // Create Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Assign Fill to Slider
        slider.fillRect = fillRect;

        // Create Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderObject.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);

        // Assign Handle to Slider
        slider.targetGraphic = handleImage;
        slider.handleRect = handleRect;

        // Set Slider RectTransform
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = anchorPosition;
        sliderRect.anchorMax = anchorPosition;
        sliderRect.sizeDelta = new Vector2(200, 20);
        sliderRect.anchoredPosition = Vector2.zero;

        // Create Label
        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(sliderObject.transform, false);
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 24;
        label.color = Color.black;
        label.alignment = TextAlignmentOptions.Center;
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 1f);
        labelRect.anchorMax = new Vector2(0.5f, 1f);
        labelRect.sizeDelta = new Vector2(200, 30);
        labelRect.anchoredPosition = new Vector2(0, 15);

        return slider;
    }

    private Button CreateButton(string name, string text, Vector2 anchorPosition)
    {
        // Create Button GameObject
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(settingsCanvas.transform, false);

        // Add Image component
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.8f, 0.8f, 0.8f); // Light gray color

        // Add Button component
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        // Create Text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.color = Color.black;
        buttonText.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        // Set Button RectTransform
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorPosition;
        buttonRect.anchorMax = anchorPosition;
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = Vector2.zero;

        return button;
    }

    public void SetBrightness(float value)
    {
        // Adjust brightness
        RenderSettings.ambientLight = Color.white * value;
        PlayerPrefs.SetFloat("Brightness", value);
    }

    public void SetAudio(float value)
    {
        // Adjust audio volume
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Audio", value);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    public void ShowSettingsMenu(bool show)
    {
        settingsCanvas.SetActive(show);
    }

    private void SaveAndClose()
    {
        SaveSettings();
        ShowSettingsMenu(false);
        if (FindObjectOfType<PauseScreenUI>() != null)
        {
            FindObjectOfType<PauseScreenUI>().ShowPauseMenu(true);
        }
        else
        {
            FindObjectOfType<StartScreen>().ShowStartScreen(true);
        }
    }

    private void CloseSettingsMenu()
    {
        ShowSettingsMenu(false);
        if (FindObjectOfType<PauseScreenUI>() != null)
        {
            FindObjectOfType<PauseScreenUI>().ShowPauseMenu(true);
        }
        else
        {
            FindObjectOfType<StartScreen>().ShowStartScreen(true);
        }
    }
}