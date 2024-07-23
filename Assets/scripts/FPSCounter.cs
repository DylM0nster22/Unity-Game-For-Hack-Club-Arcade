using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;

    private float accum = 0f;
    private int frames = 0;
    private float timeleft;
    private TextMeshProUGUI fpsText;

    void Start()
    {
        GameObject fpsObject = new GameObject("FPSText");
        fpsObject.transform.SetParent(transform, false);
        fpsText = fpsObject.AddComponent<TextMeshProUGUI>();
        fpsText.fontSize = 24;
        fpsText.color = Color.yellow;
        fpsText.alignment = TextAlignmentOptions.TopRight;

        RectTransform textRect = fpsText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.anchoredPosition = new Vector2(-10, -10);
        textRect.sizeDelta = new Vector2(200, 50);

        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            fpsText.text = format;

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    public void ShowFPSCounter(bool show)
    {
        fpsText.enabled = show;
    }
}