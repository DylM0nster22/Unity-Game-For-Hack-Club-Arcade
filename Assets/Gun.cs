using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed = 20f;
    public int magazineSize = 10;
    public float reloadTime = 2f;
    public ParticleSystem muzzleFlash;
    public int damageAmount = 10; // Adjust the damage amount as needed

    private int bulletsLeft;
    private bool isReloading = false;

    void Start()
    {
        bulletsLeft = magazineSize;
        if (muzzleFlash == null)
        {
            muzzleFlash = GetComponentInChildren<ParticleSystem>();
        }
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetButtonDown("Fire1") && bulletsLeft > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (bulletsLeft <= 0) return;

        // Instantiate the bullet for visual effect
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = bulletSpawn.forward * bulletSpeed;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        bulletsLeft--;

        // Perform raycast for hit detection
        RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit))
        {
            Health healthComponent = hit.transform.GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damageAmount);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = magazineSize;
        isReloading = false;
    }
}