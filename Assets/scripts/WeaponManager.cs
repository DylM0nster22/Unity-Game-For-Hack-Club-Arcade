using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public List<WeaponController> weapons;
    private Image[] weaponSlots;
    
    private int currentWeaponIndex = 0;

    [Header("UI Settings")]
    public Canvas weaponSlotsCanvas;
    public int slotCount = 5;
    public float slotSize = 80f;
    public float spacing = 10f;
    public Vector2 startPosition = new Vector2(10f, 10f);

    void Start()
    {
        CreateWeaponSlots();

        // Disable all weapons except the first one
        for (int i = 1; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }
        UpdateWeaponSlots();
    }

    void CreateWeaponSlots()
    {
        if (weaponSlotsCanvas == null)
        {
            Debug.LogError("Weapon Slots Canvas not assigned to WeaponManager!");
            return;
        }

        weaponSlots = new Image[slotCount];
        
        // Create a parent object for our weapon slots
        GameObject slotsParent = new GameObject("WeaponSlots");
        RectTransform slotsParentRect = slotsParent.AddComponent<RectTransform>();
        slotsParentRect.SetParent(weaponSlotsCanvas.transform, false);

        // Position the parent object in the bottom-right corner
        slotsParentRect.anchorMin = new Vector2(1, 0);
        slotsParentRect.anchorMax = new Vector2(1, 0);
        slotsParentRect.anchoredPosition = new Vector2(-startPosition.x, startPosition.y);

        // Calculate total width
        float totalWidth = (slotCount * slotSize) + ((slotCount - 1) * spacing);

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = new GameObject($"WeaponSlot_{i}");
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.SetParent(slotsParentRect, false);

            // Set size and position
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);
            float xPos = (i * (slotSize + spacing)) - (totalWidth - slotSize);
            slotRect.anchoredPosition = new Vector2(xPos, 0);

            // Add Image component
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = Color.gray; // Default color

            weaponSlots[i] = slotImage;
        }
    }

    void Update()
    {
        // Check for number key inputs (1-5)
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < weapons.Count)
            {
                SwitchWeapon(i);
                break;
            }
        }

        // Add mouse wheel scrolling for weapon switching
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel > 0f)
        {
            SwitchWeapon((currentWeaponIndex + 1) % weapons.Count);
        }
        else if (scrollWheel < 0f)
        {
            SwitchWeapon((currentWeaponIndex - 1 + weapons.Count) % weapons.Count);
        }
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex != currentWeaponIndex && newIndex < weapons.Count)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(false);
            weapons[newIndex].gameObject.SetActive(true);
            currentWeaponIndex = newIndex;
            UpdateWeaponSlots();
        }
    }

    void UpdateWeaponSlots()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i < weapons.Count)
            {
                weaponSlots[i].sprite = weapons[i].weaponIcon;
                weaponSlots[i].color = i == currentWeaponIndex ? Color.yellow : Color.white;
            }
            else
            {
                weaponSlots[i].sprite = null;
                weaponSlots[i].color = Color.gray;
            }
        }
    }
}