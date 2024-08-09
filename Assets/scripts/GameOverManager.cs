using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject startScreen; // Reference to the Start Screen GameObject
    private Canvas gameOverCanvas;
    private bool isGameOver = false;

    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public WeaponController weaponController;

    void Start()
    {
        CreateGameOverCanvas();
        gameOverCanvas.gameObject.SetActive(false);

        // Find and assign player scripts
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();

        PlayerMovement.OnPlayerDeath += ShowGameOverScreen;
    }

    void OnDestroy()
    {
        PlayerMovement.OnPlayerDeath -= ShowGameOverScreen;
    }

    void CreateGameOverCanvas()
    {
        GameObject canvasObject = new GameObject("GameOverCanvas");
        gameOverCanvas = canvasObject.AddComponent<Canvas>();
        gameOverCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameOverCanvas.sortingOrder = 100; // Ensure it's on top of other UI elements
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        CreateBackground();
        CreateTitle();
        CreateQuitButton();
    }

    void CreateBackground()
    {
        GameObject backgroundObject = new GameObject("GameOverBackground");
        backgroundObject.transform.SetParent(gameOverCanvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
    }

    void CreateTitle()
    {
        GameObject titleObject = new GameObject("GameOverTitleText");
        titleObject.transform.SetParent(gameOverCanvas.transform, false);
        TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "Game Over";
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.sizeDelta = new Vector2(300, 60);
        titleRect.anchoredPosition = Vector2.zero;
    }

    void CreateQuitButton()
    {
        CreateButton("QuitButton", "Quit", new Vector2(0.5f, 0.3f), QuitGame);
        CreateButton("TitleButton", "Back to Title", new Vector2(0.5f, 0.45f), BackToTitle);
    }

    void CreateButton(string name, string text, Vector2 anchorPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(gameOverCanvas.transform, false);
        
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

    void ShowGameOverScreen()
    {
        isGameOver = true;
        gameOverCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        DisablePlayerInput();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void DisablePlayerInput()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerShooting != null)
            playerShooting.enabled = false;
        if (weaponController != null)
            weaponController.enabled = false;
    }

    void EnablePlayerInput()
    {
        // Reset player health, score, and any other relevant game state variables
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerShooting != null)
            playerShooting.enabled = true;
        if (weaponController != null)
            weaponController.enabled = true;
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void BackToTitle()
    {
        Debug.Log("Returning to title screen");
        Time.timeScale = 1f; // Ensure the game is unpaused

        // Reset the player's health and position
        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();
        }

        // Despawn all enemies
        DespawnAllEnemies();

        // Reset the enemy waves
        if (GameController.Instance != null && GameController.Instance.waveEnemySpawner != null)
        {
            GameController.Instance.waveEnemySpawner.ResetWaves();
        }

        // Disable the Game Over UI
        gameOverCanvas.gameObject.SetActive(false);

        // Enable and show the Start Screen
        StartScreen startScreen = FindObjectOfType<StartScreen>();
        if (startScreen != null)
        {
            startScreen.ShowStartScreen(true);
        }
        else
        {
            Debug.LogError("StartScreen reference is missing in GameOverManager!");
        }

        // Disable player input
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    void DespawnAllEnemies()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}