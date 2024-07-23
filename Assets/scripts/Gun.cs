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
    public FireMode fireMode = FireMode.Single; // New: Fire mode selection

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce;
    public float verticalRecoil = 0.1f; // New: Vertical recoil
    public float horizontalRecoil = 0.05f; // New: Horizontal recoil

    [Header("References")]
    public Camera playerCamera;
    public Transform firePoint;

    [Header("Graphics")]
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    [Header("Enemy Interaction")]
    public float enemyKnockbackForce = 2f;

    [Header("UI")]
    public Sprite weaponIcon; // Added this line

    [Header("Audio")]
    public AudioClip shootSound; // New: Sound effect for shooting
    public AudioClip reloadSound; // New: Sound effect for reloading
    private AudioSource audioSource;

    private int bulletsRemaining;
    private int bulletsFired;
    private bool isShooting, canShoot, isReloading;
    private float nextFireTime = 0f;

    private void Awake()
    {
        bulletsRemaining = magazineCapacity;
        canShoot = true;

        SetupAmmoDisplay();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f) // Game is paused
            return;

        HandleInput();
        UpdateAmmoDisplay();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) // Game is paused
            return;
    }

    private void HandleInput()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                isShooting = Input.GetButtonDown("Fire1");
                break;
            case FireMode.Burst:
                if (Input.GetButtonDown("Fire1") && !isShooting)
                {
                    StartCoroutine(BurstFire());
                }
                break;
            case FireMode.Auto:
                isShooting = Input.GetButton("Fire1");
                break;
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineCapacity && !isReloading) Reload();
        if (canShoot && isShooting && !isReloading && bulletsRemaining <= 0) Reload();

        if (canShoot && isShooting && !isReloading && bulletsRemaining > 0 && Time.time >= nextFireTime)
        {
            bulletsFired = 0;
            Fire();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < 3; i++)
        {
            if (canShoot && !isReloading && bulletsRemaining > 0)
            {
                Fire();
                yield return new WaitForSeconds(timeBetweenBullets);
            }
            else
            {
                break;
            }
        }
    }

    private void Fire()
    {
        canShoot = false;

        // Play shoot sound once at the beginning of the Fire method
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        for (int i = 0; i < bulletsPerShot; i++)
        {
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
        }

        if (muzzleFlash != null) Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);

        bulletsRemaining--;
        bulletsFired++;

        UpdateAmmoDisplay();

        ApplyRecoil();

        Invoke(nameof(ResetShot), timeBetweenShots);
    }

    private void ApplyRecoil()
    {
        playerRb.AddForce(-playerCamera.transform.forward * recoilForce, ForceMode.Impulse);
        playerCamera.transform.Rotate(Vector3.right * verticalRecoil);
        playerCamera.transform.Rotate(Vector3.up * Random.Range(-horizontalRecoil, horizontalRecoil));
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

    public void Reload()
    {
        if (!isReloading && bulletsRemaining < magazineCapacity)
        {
            isReloading = true;
            if (reloadSound != null)
            {
                audioSource.PlayOneShot(reloadSound);
            }
            Invoke(nameof(FinishReloading), reloadDuration);
        }
    }

    private void FinishReloading()
    {
        bulletsRemaining = magazineCapacity;
        isReloading = false;
        UpdateAmmoDisplay();
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
            ammoDisplay.SetText($"{bulletsRemaining} / {magazineCapacity}");
        }
    }

    public enum FireMode
    {
        Single,
        Burst,
        Auto
    }
}