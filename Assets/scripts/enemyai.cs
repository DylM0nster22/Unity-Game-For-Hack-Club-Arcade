using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navAgent;

    public Transform player;

    public LayerMask groundLayer, playerLayer;

    public float health;

    //Patrolling
    public Vector3 patrolPoint;
    bool patrolPointSet;
    public float patrolRange;

    //Attacking
    public float attackInterval;
    bool hasAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        navAgent = GetComponent<NavMeshAgent>();
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
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
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
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void Respawn()
    {
        // Reset health
        health = 100; // or whatever the default health value is

        // Reset position or other necessary states
        patrolPointSet = false;
        hasAttacked = false;
        // Optionally, you can reset the position to a spawn point
        // transform.position = spawnPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}