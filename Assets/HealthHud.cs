using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }

        // Position the health bar in the top right corner
        RectTransform healthBarRect = healthBarBackground.rectTransform;
        healthBarRect.anchorMin = new Vector2(1, 1);
        healthBarRect.anchorMax = new Vector2(1, 1);
        healthBarRect.anchoredPosition = new Vector2(-10, -10);
        healthBarRect.sizeDelta = new Vector2(200, 20);

        // Set up the health bar fill
        RectTransform fillRect = healthBarFill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Position the health text
        RectTransform healthTextRect = healthText.rectTransform;
        healthTextRect.anchorMin = new Vector2(1, 1);
        healthTextRect.anchorMax = new Vector2(1, 1);
        healthTextRect.anchoredPosition = new Vector2(-10, -35);
    }

    void Update()
    {
        if (playerController != null)
        {
            UpdateHealthBar(playerController.health);
        }
    }

    void UpdateHealthBar(int currentHealth)
    {
        float healthPercentage = (float)currentHealth / playerController.maxHealth;
        healthBarFill.fillAmount = healthPercentage;
        
        // Interpolate color between low health and full health
        healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);

        healthText.text = $"Health: {currentHealth}";
    }
}