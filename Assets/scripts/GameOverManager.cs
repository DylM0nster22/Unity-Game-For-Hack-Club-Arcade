using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private GameObject gameOverScreen;
    private Button respawnButton;
    private Button quitButton;

    private PlayerController player;
    private Canvas canvas;
    public Font customFont; // Assign this in the Unity Inspector

    void Start()
    {
        Debug.Log("GameOverManager Start");
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController not found!");
        }
        CreateGameOverScreen();

        PlayerController.OnPlayerDeath += ShowGameOverScreen;
        Debug.Log("GameOverManager initialized");
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerDeath -= ShowGameOverScreen;
    }

    void CreateGameOverScreen()
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("GameOverCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Create game over panel
        gameOverScreen = CreatePanel();
        gameOverScreen.SetActive(false);

        // Create "Game Over" text
        CreateText("Game Over", new Vector2(0, 100));

        // Create Respawn button
        respawnButton = CreateButton("Respawn", new Vector2(0, 0), Respawn);

        // Create Quit button
        quitButton = CreateButton("Quit", new Vector2(0, -100), Quit);
    }

    GameObject CreatePanel()
    {
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);

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
        GameObject textObject = new GameObject("GameOverText");
        textObject.transform.SetParent(gameOverScreen.transform, false);

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
        buttonObject.transform.SetParent(gameOverScreen.transform, false);

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

    void ShowGameOverScreen()
    {
        Debug.Log("Showing Game Over Screen");
        gameOverScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Respawn()
    {
        gameOverScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.Respawn(); // This will now respawn all objects
    }

    void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}