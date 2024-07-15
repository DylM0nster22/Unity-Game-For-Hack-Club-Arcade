using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    private GameObject endGameScreen;
    private Button retryButton;
    private Button exitButton;

    private PlayerMovement player;
    private Canvas uiCanvas;
    public Font customFont; // Assign this in the Unity Inspector

    void Start()
    {
        Debug.Log("EndGameController Start");
        player = FindObjectOfType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found!");
        }
        CreateEndGameScreen();

        PlayerMovement.OnPlayerDeath += ShowEndGameScreen;
        Debug.Log("EndGameController initialized");
    }

    void OnDestroy()
    {
        PlayerMovement.OnPlayerDeath -= ShowEndGameScreen;
    }

    void CreateEndGameScreen()
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("EndGameCanvas");
        uiCanvas = canvasObject.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Create end game panel
        endGameScreen = CreatePanel();
        endGameScreen.SetActive(false);

        // Create "Game Over" text
        CreateText("Game Over", new Vector2(0, 100));

        // Create Retry button
        retryButton = CreateButton("Retry", new Vector2(0, 0), Retry);

        // Create Exit button
        exitButton = CreateButton("Exit", new Vector2(0, -100), Exit);
    }

    GameObject CreatePanel()
    {
        GameObject panel = new GameObject("EndGamePanel");
        panel.transform.SetParent(uiCanvas.transform, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        return panel;
    }

    void CreateText(string message, Vector2 position)
    {
        GameObject textObject = new GameObject("EndGameText");
        textObject.transform.SetParent(endGameScreen.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.text = message;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Changed from Arial.ttf
        text.fontSize = 48;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(400, 100);
    }

    Button CreateButton(string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button");
        buttonObject.transform.SetParent(endGameScreen.transform, false);

        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f);

        Text text = CreateButtonText(buttonObject, label);

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(200, 50);

        return button;
    }

    Text CreateButtonText(GameObject buttonObject, string label)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Changed from Arial.ttf
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        return text;
    }

    void ShowEndGameScreen()
    {
        Debug.Log("Showing End Game Screen");
        endGameScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Retry()
    {
        endGameScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.Respawn(); // This will now respawn all objects
    }

    void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}