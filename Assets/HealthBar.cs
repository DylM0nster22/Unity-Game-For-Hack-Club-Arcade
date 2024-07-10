using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;
    public Vector2 healthBarSize = new Vector2(100, 10); // Public variable for health bar size
    public Vector3 offset = new Vector3(0, 2, 0); // Public variable for offset
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    private GameObject healthBarCanvas;
    private RectTransform healthBarRect;
    private Text healthText;
    private Image healthBarImage;

    void Start()
    {
        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        // Create a new Canvas
        healthBarCanvas = new GameObject("HealthBarCanvas");
        healthBarCanvas.transform.SetParent(transform);
        Canvas canvas = healthBarCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 1;

        // Adjust the scale of the canvas to make it smaller
        healthBarCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Create the HealthBar background
        GameObject healthBarBg = new GameObject("HealthBarBG");
        healthBarBg.transform.SetParent(healthBarCanvas.transform);
        Image bgImage = healthBarBg.AddComponent<Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = healthBarBg.GetComponent<RectTransform>();
        bgRect.sizeDelta = healthBarSize; // Use public variable
        bgRect.anchoredPosition = Vector2.zero;

        // Create the HealthBar
        GameObject healthBar = new GameObject("HealthBar");
        healthBar.transform.SetParent(healthBarBg.transform);
        healthBarImage = healthBar.AddComponent<Image>();
        healthBarImage.color = fullHealthColor;
        healthBarRect = healthBar.GetComponent<RectTransform>();
        healthBarRect.sizeDelta = healthBarSize; // Use public variable
        healthBarRect.anchorMin = new Vector2(0, 0.5f);
        healthBarRect.anchorMax = new Vector2(0, 0.5f);
        healthBarRect.pivot = new Vector2(0, 0.5f);
        healthBarRect.anchoredPosition = Vector2.zero;

        // Create the HealthText
        GameObject healthTextObj = new GameObject("HealthText");
        healthTextObj.transform.SetParent(healthBarBg.transform);
        healthText = healthTextObj.AddComponent<Text>();
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        healthText.color = Color.white;
        RectTransform textRect = healthText.GetComponent<RectTransform>();
        textRect.sizeDelta = healthBarSize; // Use public variable
        textRect.anchoredPosition = Vector2.zero;

        // Set the initial position of the health bar canvas
        healthBarCanvas.transform.localPosition = offset;
    }

    void Update()
    {
        if (health != null && healthBarRect != null && healthText != null)
        {
            // Update the health bar position to follow the enemy with offset
            healthBarCanvas.transform.position = transform.position + offset;

            float healthPercentage = (float)health.CurrentHealth / health.maxHealth;
            healthBarRect.localScale = new Vector3(healthPercentage, 1, 1);

            healthText.text = health.CurrentHealth.ToString();

            Color healthColor = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
            healthBarImage.color = healthColor;
        }
    }
}
