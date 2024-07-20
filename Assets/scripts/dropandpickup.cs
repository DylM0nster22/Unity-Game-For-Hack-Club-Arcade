using UnityEngine;

public class DropAndPickup : MonoBehaviour
{
    public Transform player;
    public Transform gunHold;
    public float pickupRange = 2f;
    private GameObject currentGun;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupGun();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            DropGun();
        }
    }

    void TryPickupGun()
    {
        if (currentGun == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.position, player.forward, out hit, pickupRange))
            {
                if (hit.transform.CompareTag("Gun"))
                {
                    PickupGun(hit.transform.gameObject);
                }
            }
        }
    }

    void PickupGun(GameObject gun)
    {
        currentGun = gun;
        gun.transform.SetParent(gunHold);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.GetComponent<Rigidbody>().isKinematic = true;

        // Ensure the gun's world position matches the player's position
        gun.transform.position = gunHold.position;
        gun.transform.rotation = gunHold.rotation;

        // Enable the WeaponController script on the picked-up gun
        gun.GetComponent<WeaponController>().enabled = true;
    }

    void DropGun()
    {
        if (currentGun != null)
        {
            currentGun.transform.SetParent(null);
            currentGun.GetComponent<Rigidbody>().isKinematic = false;

            // Disable the WeaponController script on the dropped gun
            currentGun.GetComponent<WeaponController>().enabled = false;

            currentGun = null;
        }
    }
}