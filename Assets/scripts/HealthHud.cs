using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    [Header("Health Bar Settings")]
    public float healthBarWidth = 200f;
    public float healthBarHeight = 20f;
    public float margin = 10f;

    private PlayerMovement playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerMovement>();
        
        if (playerController == null)
        {
            Debug.LogError("PlayerMovement not found in the scene!");
        }

        SetupHealthBar();
        UpdateHealthBar(playerController.currentHealth);
    }

    void SetupHealthBar()
    {
        // Position the health bar in the bottom left corner
        RectTransform healthBarRect = healthBarBackground.rectTransform;
        healthBarRect.anchorMin = new Vector2(0, 0);
        healthBarRect.anchorMax = new Vector2(0, 0);
        healthBarRect.anchoredPosition = new Vector2(margin + healthBarWidth / 2, margin + healthBarHeight / 2);
        healthBarRect.sizeDelta = new Vector2(healthBarWidth, healthBarHeight);

        // Set up the health bar fill
        RectTransform fillRect = healthBarFill.rectTransform;
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;

        // Position the health text
        RectTransform healthTextRect = healthText.rectTransform;
        healthTextRect.anchorMin = new Vector2(0, 0);
        healthTextRect.anchorMax = new Vector2(0, 0);
        healthTextRect.anchoredPosition = new Vector2(margin + healthBarWidth / 2, margin + healthBarHeight + 5);
        healthTextRect.sizeDelta = new Vector2(healthBarWidth, 20);
    }

    void Update()
    {
        if (playerController != null)
        {
            UpdateHealthBar(playerController.currentHealth);
        }
    }

    public void SetHealth(int currentHealth)
    {
        UpdateHealthBar(currentHealth);
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