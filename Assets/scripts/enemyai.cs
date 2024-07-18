using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    // Reference to the HealthBar script
    public HealthBar healthBar;

    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;
    public float respawnDelay = 5f; // Time in seconds before the enemy respawns

    //Patrolling
    public Vector3 patrolPoint;
    bool patrolPointSet;
    public float patrolRange;

    //Attacking
    public float attackInterval;
    bool hasAttacked;
    public GameObject projectilePrefab; // Change this from Rigidbody to GameObject
    public int projectileDamage = 10;
    public float shootingForce = 32f;
    public float projectileSpeed = 20f; // Add this line

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Transform modelTransform; // Assign this in the inspector to your model's transform
    private Quaternion desiredRotation;

    private Health healthComponent;

    private Vector3 initialPosition;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        navAgent = GetComponent<NavMeshAgent>();

        // Initialize health
        currentHealth = maxHealth;

        // Create a Health component and link it to the HealthBar
        healthComponent = gameObject.AddComponent<Health>();
        healthComponent.MaxHealth = maxHealth;
        healthComponent.CurrentHealth = currentHealth;

        if (healthBar != null)
        {
            healthBar.SetHealthComponent(healthComponent);
        }

        // Store the desired rotation
        desiredRotation = modelTransform.rotation;

        // Disable NavMeshAgent rotation
        if (navAgent != null)
        {
            navAgent.updateRotation = false;
        }

        // Store the initial position
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Check if the game is paused
        if (Time.timeScale == 0f)
            return;

        // Check if the enemy has died
        if (!isDead && currentHealth <= 0)
        {
            HandleDeath();
            return;
        }

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) FollowPlayer();
        if (playerInAttackRange && playerInSightRange) EngagePlayer();

        // Constantly enforce the desired rotation
        modelTransform.rotation = desiredRotation;

        // If you need to rotate the enemy to face the player, rotate only on the Y-axis
        if (playerInSightRange || playerInAttackRange)
        {
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0; // This ensures rotation only on the Y-axis
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }

    private void Patrol()
    {
        if (!patrolPointSet) SearchPatrolPoint();

        if (patrolPointSet)
            navAgent.SetDestination(patrolPoint);

        Vector3 distanceToPatrolPoint = transform.position - patrolPoint;

        //Patrol point reached
        if (distanceToPatrolPoint.magnitude < 1f)
            patrolPointSet = false;
    }

    private void SearchPatrolPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-patrolRange, patrolRange);
        float randomX = Random.Range(-patrolRange, patrolRange);

        patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(patrolPoint, -transform.up, 2f, groundLayer))
            patrolPointSet = true;
    }

    private void FollowPlayer()
    {
        navAgent.SetDestination(player.position);
    }

    private void EngagePlayer()
    {
        // Use LookAt with the up vector to maintain upright position
        Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPosition, Vector3.up);

        if (!hasAttacked)
        {
            ShootAtPlayer();
            hasAttacked = true;
            Invoke(nameof(ResetAttack), attackInterval);
        }
    }

    private void ShootAtPlayer()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position + targetDirection, Quaternion.LookRotation(targetDirection));
        EnemyProjectile enemyProjectile = bullet.GetComponent<EnemyProjectile>();
        
        if (enemyProjectile != null)
        {
            enemyProjectile.damage = projectileDamage;
            enemyProjectile.speed = projectileSpeed;
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    public void ReceiveDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            UpdateHealthComponent();
            Debug.Log("Enemy received " + damage + " damage. Current health: " + currentHealth);

            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }
    }

    private void UpdateHealthComponent()
    {
        if (healthComponent != null)
        {
            healthComponent.CurrentHealth = currentHealth;
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        // Disable the enemy temporarily
        gameObject.SetActive(false);

        // Schedule respawn
        Invoke(nameof(Respawn), respawnDelay);
    }

    public void Respawn()
    {
        // Reset health
        currentHealth = maxHealth;
        UpdateHealthComponent();

        isDead = false;

        // Re-enable the enemy
        gameObject.SetActive(true);

        // Reset position to the initial position
        transform.position = initialPosition;

        // ... any other respawn logic ...
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}