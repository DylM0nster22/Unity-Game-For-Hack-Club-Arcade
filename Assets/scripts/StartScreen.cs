using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject startScreenPanel;
    public GameObject settingsMenu;

    private void Start()
    {
        // Pause the game when the start screen is active
        Time.timeScale = 0f;
        ShowStartScreen(true);

        // Disable player input
        DisablePlayerInput();

        // Disable enemy AI
        DisableEnemyAI(); // Ensure enemy AI is disabled when the start screen is active
    }

    public void ShowStartScreen(bool show)
    {
        startScreenPanel.SetActive(show);
        if (show)
        {
            // Mute all audio sources
            MuteAllAudio();

            // Pause the game when the start screen is active
            Time.timeScale = 0f;

            // Disable player input
            DisablePlayerInput();

            // Disable enemy AI
            DisableEnemyAI();
        }
        else
        {
            // Unmute all audio sources when starting the game
            UnmuteAllAudio();
        }
    }

    private void MuteAllAudio()
    {
        AudioListener.volume = 0f; // Mute audio
    }

    private void UnmuteAllAudio()
    {
        AudioListener.volume = 1f; // Restore audio
    }

    public void StartGame()
    {
        // Unpause the game and hide the start screen
        Time.timeScale = 1f;
        ShowStartScreen(false);

        // Enable player input
        EnablePlayerInput();

        // Enable enemy AI
        EnableEnemyAI();

        // Unmute all audio sources when starting the game
        UnmuteAllAudio();

        // You might want to load your main game scene here
        // SceneManager.LoadScene("MainGameScene");
    }

    private void DisablePlayerInput()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false; // Disable player movement
        }
    }

    private void EnablePlayerInput()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void DisableEnemyAI()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = false; // Disable enemy AI
        }
    }

    private void EnableEnemyAI()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = true; // Enable each enemy's AI
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Settings screen opened");
        if (settingsMenu != null)
        {
            ShowStartScreen(false);
            settingsMenu.GetComponent<SettingsMenu>().ShowSettingsMenu(true);
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