using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 20f;
    public float shootingRange = 10f;
    public float movementSpeed = 3f;
    public float rotationSpeed = 5f;
    public float shootingCooldown = 1f;

    private Transform player;
    private NavMeshAgent agent;
    private Gun gun;
    private Health health;
    private float lastShotTime;
    public Transform gunPivot;
    public Vector3 gunForwardDirection = Vector3.forward;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        gun = GetComponentInChildren<Gun>();
        health = GetComponent<Health>();

        if (gun != null)
        {
            gun.isPlayerGun = false; // Set this to false for enemy gun
        }

        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        if (gunPivot == null)
        {
            gunPivot = gun.transform;
        }

        lastShotTime = -shootingCooldown; // Allow immediate shooting
    }

    void Update()
    {
        if (player == null || health.CurrentHealth <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Look at player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Rotate gun to face player
            Vector3 gunDirection = player.position - gunPivot.position;
            Quaternion targetRotation = Quaternion.LookRotation(gunDirection, Vector3.up) * Quaternion.FromToRotation(Vector3.forward, gunForwardDirection);
            gunPivot.rotation = Quaternion.Slerp(gunPivot.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (distanceToPlayer <= shootingRange)
            {
                // Stop moving and shoot
                agent.isStopped = true;
                if (Time.time - lastShotTime >= shootingCooldown)
                {
                    Shoot();
                }
            }
            else
            {
                // Move towards player
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Stop moving if player is out of detection range
            agent.isStopped = true;
        }
    }

    void Shoot()
    {
        if (gun != null)
        {
            gun.Shoot();
            lastShotTime = Time.time;
        }
    }
}