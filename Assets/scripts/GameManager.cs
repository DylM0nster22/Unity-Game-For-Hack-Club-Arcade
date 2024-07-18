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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnAllEntities();
        }
    }

    public void RespawnAllEntities()
    {
        // Respawn all EnemyController objects
        EnemyController[] enemies = FindObjectsOfType<EnemyController>(true);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Respawn();
        }

        // Add any other object types that need respawning
    }
}