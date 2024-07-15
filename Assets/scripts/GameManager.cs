using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

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

    public void RespawnAllEntities()
    {
        // Respawn all Health objects
        Health[] healthObjects = FindObjectsOfType<Health>(true);
        foreach (Health health in healthObjects)
        {
            health.Respawn();
        }

        // Respawn all EnemyController objects
        EnemyController[] enemies = FindObjectsOfType<EnemyController>(true);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Respawn();
        }

        // Add any other object types that need respawning
    }
}