using UnityEngine;
using TMPro;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float shootForce = 20f;
    public float upwardForce;
    public int bulletDamage = 10; // Added this line

    [Header("Gun Stats")]
    public float timeBetweenShots = 0.5f;
    public float spread;
    public float reloadDuration;
    public float timeBetweenBullets;
    public int magazineCapacity = 30;
    public int bulletsPerShot = 1;
    public bool allowButtonHold;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce;

    [Header("References")]
    public Camera playerCamera;
    public Transform firePoint;

    [Header("Graphics")]
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    [Header("Enemy Interaction")]
    public float enemyKnockbackForce = 2f;

    private int bulletsRemaining;
    private int bulletsFired;
    private bool isShooting, canShoot, isReloading;
    private float nextFireTime = 0f;

    private void Awake()
    {
        bulletsRemaining = magazineCapacity;
        canShoot = true;

        SetupAmmoDisplay();
    }

    private void Update()
    {
        HandleInput();
        UpdateAmmoDisplay();
    }

    private void HandleInput()
    {
        if (allowButtonHold) isShooting = Input.GetButton("Fire1");
        else isShooting = Input.GetButtonDown("Fire1");

        if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineCapacity && !isReloading) Reload();
        if (canShoot && isShooting && !isReloading && bulletsRemaining <= 0) Reload();

        if (canShoot && isShooting && !isReloading && bulletsRemaining > 0 && Time.time >= nextFireTime)
        {
            bulletsFired = 0;
            Fire();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    private void Fire()
    {
        canShoot = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - firePoint.position;
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0
        );

        GameObject currentBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(directionWithSpread));
        PlayerProjectile playerProjectile = currentBullet.GetComponent<PlayerProjectile>();
        
        if (playerProjectile != null)
        {
            playerProjectile.damage = bulletDamage;
            playerProjectile.speed = shootForce;
        }
        else
        {
            Debug.LogWarning("PlayerProjectile component not found on bullet prefab!");
        }

        if (muzzleFlash != null) Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);

        bulletsRemaining--;
        bulletsFired++;

        if (bulletsFired < bulletsPerShot && bulletsRemaining > 0)
            Invoke(nameof(Fire), timeBetweenBullets);

        playerRb.AddForce(-playerCamera.transform.forward * recoilForce, ForceMode.Impulse);

        Invoke(nameof(ResetShot), timeBetweenShots);
    }

    private void ApplyEnemyKnockback(EnemyController enemy)
    {
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            knockbackDirection.y = 0;
            enemyRb.AddForce(knockbackDirection * enemyKnockbackForce, ForceMode.Impulse);
            StartCoroutine(LimitEnemyVelocity(enemyRb));
        }
    }

    private IEnumerator LimitEnemyVelocity(Rigidbody enemyRb)
    {
        yield return new WaitForFixedUpdate();
        float maxVelocity = 5f;
        if (enemyRb.linearVelocity.magnitude > maxVelocity)
        {
            enemyRb.linearVelocity = enemyRb.linearVelocity.normalized * maxVelocity;
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }

    private void Reload()
    {
        isReloading = true;
        Invoke(nameof(FinishReloading), reloadDuration);
    }

    private void FinishReloading()
    {
        bulletsRemaining = magazineCapacity;
        isReloading = false;
    }

    private void SetupAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            RectTransform rectTransform = ammoDisplay.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10);
            rectTransform.pivot = new Vector2(1, 1);
        }
    }

    private void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText($"{bulletsRemaining / bulletsPerShot} / {magazineCapacity / bulletsPerShot}");
        }
    }
}