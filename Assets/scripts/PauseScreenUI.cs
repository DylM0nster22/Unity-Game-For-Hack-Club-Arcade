using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PauseScreenUI : MonoBehaviour
{
    private Canvas pauseCanvas;
    private bool isPaused = false;
    
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public WeaponController weaponController;
    public GameObject settingsMenu; // Reference to the settings menu

    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    private GraphicRaycaster graphicRaycaster;

    public Button settingsButton;
    public Button restartButton;

    void Start()
    {
        CreatePauseCanvas();
        pauseCanvas.gameObject.SetActive(false);

        // Find and assign player scripts
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();
        if (weaponController == null)
            weaponController = FindObjectOfType<WeaponController>();

        Debug.Log("PlayerMovement: " + (playerMovement != null));
        Debug.Log("PlayerShooting: " + (playerShooting != null));
        Debug.Log("WeaponController: " + (weaponController != null));

        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem = gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
        }
        graphicRaycaster = pauseCanvas.GetComponent<GraphicRaycaster>();

        // Ensure the settings menu is initially hidden
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("SettingsMenu is not assigned.");
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }
        else
        {
            Debug.LogWarning("SettingsButton is not assigned.");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("RestartButton is not assigned.");
        }
    }

    void Update()
    {
        // Check for the Escape key to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void CreatePauseCanvas()
    {
        GameObject canvasObject = new GameObject("PauseCanvas");
        pauseCanvas = canvasObject.AddComponent<Canvas>();
        pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pauseCanvas.sortingOrder = 100; // Ensure it's on top of other UI elements
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        CreateBackground();
        CreateTitle();
        CreateResumeButton();
        restartButton = CreateButton("RestartButton", "Restart", new Vector2(0.5f, 0.45f), RestartGame);
        settingsButton = CreateButton("SettingsButton", "Settings", new Vector2(0.5f, 0.3f), OpenSettings);
        CreateQuitButton();
    }

    void CreateBackground()
    {
        GameObject backgroundObject = new GameObject("PauseBackground");
        backgroundObject.transform.SetParent(pauseCanvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
    }

    void CreateTitle()
    {
        GameObject titleObject = new GameObject("PauseTitleText");
        titleObject.transform.SetParent(pauseCanvas.transform, false);
        TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "Paused";
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.sizeDelta = new Vector2(300, 60);
        titleRect.anchoredPosition = Vector2.zero;
    }

    void CreateResumeButton()
    {
        CreateButton("ResumeButton", "Resume", new Vector2(0.5f, 0.6f), Resume);
    }

    void CreateQuitButton()
    {
        CreateButton("QuitButton", "Quit", new Vector2(0.5f, 0.15f), QuitGame);
    }

    Button CreateButton(string name, string text, Vector2 anchorPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(pauseCanvas.transform, false);
        
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

        return button;
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

    void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.gameObject.SetActive(isPaused);
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            DisablePlayerInput();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            EnablePlayerInput();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerShooting != null)
            playerShooting.enabled = true;
        if (weaponController != null)
            weaponController.enabled = true;
    }

    void HandlePauseMenuInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    break;
                }
            }
        }
    }

    void Resume()
    {
        TogglePause();
    }

    void RestartGame()
    {
        Debug.Log("Restart Game");
        Time.timeScale = 1f;
        
        // Reset the enemy waves
        if (GameController.Instance.waveEnemySpawner != null)
        {
            GameController.Instance.waveEnemySpawner.ResetWaves();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OpenSettings()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
            pauseCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Settings menu is null");
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowPauseMenu(bool show)
    {
        pauseCanvas.gameObject.SetActive(show);
        isPaused = show;

        if (show)
        {
            Time.timeScale = 0f;
            DisablePlayerInput();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            EnablePlayerInput();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}