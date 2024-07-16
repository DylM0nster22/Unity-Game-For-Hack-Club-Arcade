using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;
    public float fireRate = 0.5f; // Time between shots
    public Camera playerCamera;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100); // Default range
        }

        Vector3 shootDirection = targetPoint - shootPoint.position;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Ensure the projectile does not collide with the player
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        // Apply force to the projectile
        projectileRb.AddForce(shootDirection.normalized * shootForce, ForceMode.Impulse);
    }
}