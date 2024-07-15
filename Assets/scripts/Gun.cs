using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    //bullet 
    public GameObject bulletPrefab;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShots, spread, reloadDuration, timeBetweenBullets;
    public int magazineCapacity, bulletsPerShot;
    public bool allowButtonHold;

    int bulletsRemaining, bulletsFired;

    //Recoil
    public Rigidbody playerRb;
    public float recoilForce;

    //bools
    bool isShooting, canShoot, isReloading;

    //Reference
    public Camera fpsCam;
    public Transform firePoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    //bug fixing :D
    public bool allowInvoke = true;

    private void Awake()
    {
        //make sure magazine is full
        bulletsRemaining = magazineCapacity;
        canShoot = true;

        // Position ammo display in top right corner
        if (ammoDisplay != null)
        {
            RectTransform rectTransform = ammoDisplay.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10);
            rectTransform.pivot = new Vector2(1, 1);
        }
    }

    private void Update()
    {
        HandleInput();

        //Set ammo display, if it exists :D
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsRemaining / bulletsPerShot + " / " + magazineCapacity / bulletsPerShot);
        }
    }
    private void HandleInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) isShooting = Input.GetKey(KeyCode.Mouse0);
        else isShooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineCapacity && !isReloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (canShoot && isShooting && !isReloading && bulletsRemaining <= 0) Reload();

        //Shooting
        if (canShoot && isShooting && !isReloading && bulletsRemaining > 0)
        {
            //Set bullets fired to 0
            bulletsFired = 0;

            Fire();
        }
    }

    private void Fire()
    {
        canShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from firePoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - firePoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add BulletLifetime component if it doesn't exist
        if (currentBullet.GetComponent<BulletLifetime>() == null)
        {
            currentBullet.AddComponent<BulletLifetime>();
        }

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);

        bulletsRemaining--;
        bulletsFired++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShots
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShots);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerShot make sure to repeat fire function
        if (bulletsFired < bulletsPerShot && bulletsRemaining > 0)
            Invoke("Fire", timeBetweenBullets);

        // Check if the bullet hits an object with health
        if (hit.collider != null)
        {
            Health Health = hit.collider.GetComponent<Health>();
            if (Health != null)
            {
                Health.TakeDamage(10); // Adjust damage value as needed
            }
        }
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        canShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        isReloading = true;
        Invoke("FinishReloading", reloadDuration); //Invoke FinishReloading function with your reloadDuration as delay
    }

    private void FinishReloading()
    {
        //Fill magazine
        bulletsRemaining = magazineCapacity;
        isReloading = false;
    }
}