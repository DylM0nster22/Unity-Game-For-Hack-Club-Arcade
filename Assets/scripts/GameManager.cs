using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public void RespawnAllObjects()
    {
        // Respawn all Health objects
        Health[] healthObjects = FindObjectsOfType<Health>(true);
        foreach (Health health in healthObjects)
        {
            health.Respawn();
        }

        // Respawn all Enemy objects
        Enemy[] enemies = FindObjectsOfType<Enemy>(true);
        foreach (Enemy enemy in enemies)
        {
            enemy.Respawn();
        }

        // Add any other object types that need respawning
    }
}