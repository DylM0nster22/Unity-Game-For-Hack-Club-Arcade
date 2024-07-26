using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private float difficultyScalingFactor = 0.1f;
    [SerializeField] private int maxConcurrentEnemies = 15;

    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform playerTransform;
    private GameController gameController;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gameController = GameController.Instance;
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        currentWave++;
        int enemiesToSpawn = baseEnemiesPerWave + Mathf.FloorToInt(currentWave * difficultyScalingFactor);

        StartCoroutine(SpawnEnemiesOverTime(enemiesToSpawn));

        Debug.Log($"Wave {currentWave} started with {enemiesToSpawn} enemies.");
    }

    private IEnumerator SpawnEnemiesOverTime(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (activeEnemies.Count < maxConcurrentEnemies)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f); // Slight delay between spawns
            }
            else
            {
                yield return new WaitUntil(() => activeEnemies.Count < maxConcurrentEnemies);
                i--; // Retry this spawn
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.player = playerTransform;
            enemyController.OnEnemyDeath += HandleEnemyDeath;
        }
        
        activeEnemies.Add(enemy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        spawnPos.y = 1f; // Adjust this based on your enemy prefab
        return spawnPos;
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void ResetWaves()
    {
        Debug.Log("Resetting waves...");
        StopAllCoroutines();
        foreach (GameObject enemy in activeEnemies)
        {
            Debug.Log("Destroying enemy: " + enemy.name);
            Destroy(enemy);
        }
        activeEnemies.Clear();
        currentWave = 0; // Reset the wave count
        StartCoroutine(SpawnWaves()); // Restart the wave spawning
    }
}