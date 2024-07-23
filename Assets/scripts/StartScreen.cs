using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject startScreenPanel;
    public GameObject settingsMenu;
    private SettingsManager settingsManager;

    private void Start()
    {
        // Pause the game when the start screen is active
        Time.timeScale = 0f;
        ShowStartScreen(true);

        // Disable player input
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable enemy AI
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = false;
        }

        // Get the SettingsManager component
        settingsManager = settingsMenu.GetComponent<SettingsManager>();
        if (settingsManager == null)
        {
            Debug.LogError("SettingsManager component not found on settingsMenu GameObject.");
        }
    }

    public void ShowStartScreen(bool show)
    {
        startScreenPanel.SetActive(show);
        if (show)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (!settingsManager.IsSettingsMenuActive)
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void StartGame()
    {
        // Unpause the game and hide the start screen
        Time.timeScale = 1f;
        ShowStartScreen(false);

        // Enable player input
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Enable enemy AI
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = true;
        }

        // Ensure the cursor is locked and invisible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        Debug.Log("Settings screen opened");
        if (settingsManager != null)
        {
            ShowStartScreen(false);
            settingsManager.OpenSettings();
        }
        else
        {
            Debug.LogWarning("SettingsManager not assigned to StartScreen.");
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}