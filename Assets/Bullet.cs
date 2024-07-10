using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public int damage = 10; // Make sure this is set to a non-zero value

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController hitPlayer = other.GetComponent<PlayerController>();
        if (hitPlayer != null)
        {
            hitPlayer.TakeDamage(damage);
            Debug.Log($"Player hit! Damage dealt: {damage}");
        }
        else
        {
            Health hitObject = other.GetComponent<Health>();
            if (hitObject != null)
            {
                hitObject.TakeDamage(damage);
                Debug.Log($"Object hit! Damage dealt: {damage}");
            }
        }
        Destroy(gameObject);
    }
}