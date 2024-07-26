using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public WeaponController weaponController;
    public WaveEnemySpawner waveEnemySpawner; // Added this line

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
                weaponController.Reload();
            }
        }
    }

    public void RespawnAllEntities()
    {
        // Instead of respawning individual enemies, reset the wave spawner
        if (waveEnemySpawner != null)
        {
            Debug.Log("Resetting waves...");
            waveEnemySpawner.ResetWaves(); // This resets the enemy spawner to the initial state
        }

        // Add any other respawn logic here if needed
    }
}