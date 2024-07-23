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
        FindObjectOfType<PlayerMovement>().enabled = false;

        // Disable enemy AI
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = false;
        }
    }

    public void ShowStartScreen(bool show)
    {
        startScreenPanel.SetActive(show);
    }

    public void StartGame()
    {
        // Unpause the game and hide the start screen
        Time.timeScale = 1f;
        ShowStartScreen(false);

        // Enable player input
        FindObjectOfType<PlayerMovement>().enabled = true;

        // Enable enemy AI
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = true;
        }

        // You might want to load your main game scene here
        // SceneManager.LoadScene("MainGameScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings screen opened");
        if (settingsMenu != null)
        {
            ShowStartScreen(false);
            settingsMenu.GetComponent<SettingsManager>().OpenSettings();
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