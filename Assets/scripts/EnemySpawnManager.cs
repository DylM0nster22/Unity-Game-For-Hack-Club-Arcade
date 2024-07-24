using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;
    public LayerMask groundLayer;
    public float raycastDistance = 10f;

    private static EnemySpawnManager _instance;
    public static EnemySpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemySpawnManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EnemySpawnManager");
                    _instance = obj.AddComponent<EnemySpawnManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition;
        bool validPosition = false;

        do
        {
            randomPosition = new Vector3(
                Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
                spawnAreaCenter.y,
                Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
            );

            RaycastHit hit;
            if (Physics.Raycast(randomPosition + Vector3.up * raycastDistance, Vector3.down, out hit, raycastDistance * 2, groundLayer))
            {
                randomPosition = hit.point;
                validPosition = true;
            }
        } while (!validPosition);

        return randomPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}