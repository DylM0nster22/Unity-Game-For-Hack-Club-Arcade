using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public WeaponController weaponController; // Add this line

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
            if (weaponController != null)
            {
                weaponController.Reload(); // Call the reload method on the WeaponController
            }
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