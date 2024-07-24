using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public float speed = 20f;
    public float maxRange; // Added this line

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.ReceiveDamage(damage);
            Debug.Log("Dealt " + damage + " damage to enemy");
            Destroy(gameObject);
        }
    }
}