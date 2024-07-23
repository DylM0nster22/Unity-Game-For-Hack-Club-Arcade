using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float maxRange;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(initialPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject);
        }
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