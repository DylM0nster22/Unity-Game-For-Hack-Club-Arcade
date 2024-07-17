using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic; // Added this line

public class PauseScreenUI : MonoBehaviour
{
    private Canvas pauseCanvas;
    private bool isPaused = false;
    private EventSystem eventSystem;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;

    // Add references to your player scripts here
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    void Start()
    {
        CreatePauseCanvas();
        pauseCanvas.gameObject.SetActive(false);
        
        // Ensure there's an EventSystem in the scene
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }

        // Add GraphicRaycaster to the canvas
        graphicRaycaster = pauseCanvas.gameObject.AddComponent<GraphicRaycaster>();

        // Find and assign player scripts
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused)
        {
            HandleTouchInput();
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
        CreateRestartButton();
        CreateSettingsButton();
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

    void CreateRestartButton()
    {
        CreateButton("RestartButton", "Restart", new Vector2(0.5f, 0.45f), Restart);
    }

    void CreateSettingsButton()
    {
        CreateButton("SettingsButton", "Settings", new Vector2(0.5f, 0.3f), OpenSettings);
    }

    void CreateQuitButton()
    {
        CreateButton("QuitButton", "Quit", new Vector2(0.5f, 0.15f), QuitGame);
    }

    void CreateButton(string name, string text, Vector2 anchorPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(pauseCanvas.transform, false);
        
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f); // Dark gray

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
        tmpText.color = Color.white;
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
            DisablePlayerInput();
            StartCoroutine(PauseWithDelay());
        }
        else
        {
            Time.timeScale = 1f;
            EnablePlayerInput();
        }
    }

    IEnumerator PauseWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
    }

    void DisablePlayerInput()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerShooting != null)
            playerShooting.enabled = false;
    }

    void EnablePlayerInput()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerShooting != null)
            playerShooting.enabled = true;
    }

    void Resume()
    {
        TogglePause();
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OpenSettings()
    {
        Debug.Log("Open Settings");
        // Implement your settings logic here
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = touch.position;

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
    }
}