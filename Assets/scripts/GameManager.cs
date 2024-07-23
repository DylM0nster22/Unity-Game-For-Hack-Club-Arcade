using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public WeaponController weaponController;
    public SettingsManager settingsManager;
    public PauseScreenUI pauseScreenUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (settingsManager != null)
        {
            settingsManager.LoadSettings();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (weaponController != null)
            {
                weaponController.Reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void RespawnAllEntities()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>(true);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Respawn();
        }
    }

    private void TogglePause()
    {
        if (pauseScreenUI != null)
        {
            pauseScreenUI.TogglePause();
        }
    }
}