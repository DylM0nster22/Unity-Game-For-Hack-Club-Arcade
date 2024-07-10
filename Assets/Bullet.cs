using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController hitPlayer = other.GetComponent<PlayerController>();
        if (hitPlayer != null)
        {
            hitPlayer.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
