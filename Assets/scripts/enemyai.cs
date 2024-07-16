using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    // Reference to the HealthBar script
    public HealthBar healthBar;

    private Health healthComponent;

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

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        navAgent = GetComponent<NavMeshAgent>();

        // Get or add the Health component
        healthComponent = GetComponent<Health>();
        if (healthComponent == null)
        {
            healthComponent = gameObject.AddComponent<Health>();
        }

        // Link the Health component to the HealthBar
        if (healthBar != null)
        {
            healthBar.SetHealthComponent(healthComponent);
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) FollowPlayer();
        if (playerInAttackRange && playerInSightRange) EngagePlayer();
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
        //Make sure enemy doesn't move
        navAgent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!hasAttacked)
        {
            ///Attack code here
            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            EnemyProjectile enemyProjectile = bullet.GetComponent<EnemyProjectile>();
            
            if (enemyProjectile != null)
            {
                enemyProjectile.damage = projectileDamage;
            }
            
            if (rb != null)
            {
                rb.AddForce(transform.forward * shootingForce, ForceMode.Impulse);
            }
            ///End of attack code

            hasAttacked = true;
            Invoke(nameof(ResetAttack), attackInterval);
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    public void ReceiveDamage(int damage)
    {
        healthComponent.TakeDamage(damage);

        if (healthComponent.CurrentHealth <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void Respawn()
    {
        healthComponent.Respawn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}