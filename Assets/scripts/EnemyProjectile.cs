using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public float speed = 20f; // Added this line

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player!");
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Dealt " + damage + " damage to player");
            }
            else
            {
                Debug.LogWarning("Player does not have a Health component!");
            }
        }
        Destroy(gameObject);
    }
}