using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartScreenUI : MonoBehaviour
{
    private Canvas canvas;
    private StartScreen startScreen;

    void Start()
    {
        CreateCanvas();
        CreateBackground();
        CreateTitle();
        CreateStartButton();
        CreateSettingsButton();
        CreateQuitButton();

        startScreen = GetComponent<StartScreen>();
        if (startScreen == null)
        {
            startScreen = gameObject.AddComponent<StartScreen>();
        }
        startScreen.startScreenPanel = canvas.gameObject;

        // Ensure SettingsManager is created and assigned
        GameObject settingsMenuObject = GameObject.Find("SettingsManager");
        if (settingsMenuObject == null)
        {
            // Do not create a new SettingsManager if one doesn't exist
            // settingsMenuObject = new GameObject("SettingsManager");
            // settingsMenuObject.AddComponent<SettingsManager>();
        }
        startScreen.settingsMenu = settingsMenuObject;
    }

    void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("StartScreenCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    void CreateBackground()
    {
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(canvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.6f, 1f); // Light blue color
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
    }

    void CreateTitle()
    {
        GameObject titleObject = new GameObject("TitleText");
        titleObject.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "Potato Vs Pickles";
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.sizeDelta = new Vector2(500, 100);
        titleRect.anchoredPosition = Vector2.zero;
    }

    void CreateStartButton()
    {
        CreateButton("StartButton", "Start Game", new Vector2(0.5f, 0.6f), () => startScreen.StartGame());
    }

    void CreateSettingsButton()
    {
        CreateButton("SettingsButton", "Settings", new Vector2(0.5f, 0.4f), () => startScreen.OpenSettings());
    }

    void CreateQuitButton()
    {
        CreateButton("QuitButton", "Quit Game", new Vector2(0.5f, 0.2f), () => startScreen.QuitGame());
    }

    void CreateButton(string name, string text, Vector2 anchorPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(canvas.transform, false);
        
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.8f, 0.8f, 0.8f); // Light gray color

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        TextMeshProUGUI buttonText = CreateTextForButton(buttonObject, text);
        
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorPosition;
        buttonRect.anchorMax = anchorPosition;
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = Vector2.zero;

        button.onClick.AddListener(onClick);
    }

    TextMeshProUGUI CreateTextForButton(GameObject buttonObject, string text)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 24;
        tmpText.color = Color.black;
        tmpText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return tmpText;
    }
}