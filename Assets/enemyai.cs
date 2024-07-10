using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float moveSpeed = 3f;
    public Gun enemyGun;
    public Health health;
    public HealthBar healthBar;

    private bool isAttacking = false;

    void Start()
    {
        if (enemyGun == null)
        {
            enemyGun = GetComponentInChildren<Gun>();
        }
        if (health == null)
        {
            health = GetComponent<Health>();
        }
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(player);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        while (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AimAtPlayer();
            enemyGun.Shoot();
            yield return new WaitForSeconds(enemyGun.reloadTime);
        }
        isAttacking = false;
    }

    void AimAtPlayer()
    {
        Vector3 directionToPlayer = (player.position - enemyGun.bulletSpawn.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        enemyGun.transform.rotation = lookRotation;
        enemyGun.transform.Rotate(0, 90, 0); // Adjust this based on your gun's initial orientation
    }
}