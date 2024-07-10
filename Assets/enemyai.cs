using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float rotationSpeed = 5f;
    public float moveSpeed = 3f;

    private Transform player;
    private NavMeshAgent agent;
    private Gun gun;
    private Health health;
    private bool isAttacking = false;

    void Start()
    {
        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
        }

        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on the enemy. Please add a NavMeshAgent component.");
        }

        // Get Gun component
        gun = GetComponentInChildren<Gun>();
        if (gun == null)
        {
            Debug.LogError("Gun component not found in children. Make sure the enemy has a child object with the Gun script.");
        }

        // Get Health component
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Health component not found on the enemy. Please add a Health component.");
        }

        if (player != null && agent != null && gun != null && health != null)
        {
            StartCoroutine(AIBehavior());
        }
        else
        {
            Debug.LogError("EnemyAI initialization failed. Check the console for specific errors.");
        }
    }

    IEnumerator AIBehavior()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= attackRange)
                {
                    // Stop moving and attack
                    agent.isStopped = true;
                    Attack();
                }
                else
                {
                    // Move towards the player
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }

                // Rotate towards the player
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Stop moving if the player is out of range
                agent.isStopped = true;
            }

            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds for performance
        }
    }

    void Attack()
{
    if (!isAttacking)
    {
        // Face the player before attacking
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep the enemy upright
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        StartCoroutine(ShootAtPlayer());
    }
}

    IEnumerator ShootAtPlayer()
{
    isAttacking = true;

    while (Vector3.Distance(transform.position, player.position) <= attackRange)
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = player.position - gun.transform.position;
        
        // Adjust the Y component to aim at the player's center
        directionToPlayer.y += 1f; // Adjust this value based on your player's height
        
        // Normalize the direction
        directionToPlayer.Normalize();

        // Aim the gun at the player
        gun.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // Shoot
        gun.Shoot();

        yield return new WaitForSeconds(0.5f); // Wait half a second between shots
    }

    isAttacking = false;
}
}