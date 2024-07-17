using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject startScreenPanel;

    private void Start()
    {
        // Pause the game when the start screen is active
        Time.timeScale = 0f;
        ShowStartScreen();

        // Disable player input
        FindObjectOfType<PlayerMovement>().enabled = false;

        // Disable enemy AI
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.enabled = false;
        }
    }

    private void ShowStartScreen()
    {
        startScreenPanel.SetActive(true);
    }

    public void StartGame()
    {
        // Unpause the game and hide the start screen
        Time.timeScale = 1f;
        startScreenPanel.SetActive(false);

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
        // Implement settings screen logic here
        Debug.Log("Settings screen opened");
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