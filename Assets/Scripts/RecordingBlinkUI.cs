using UnityEngine;
using UnityEngine.UI;

public class RecordingBlinkUI : MonoBehaviour
{
    public float blinkInterval = 0.5f;

    private Image image;
    private float timer;

    void Start()
    {
        image = GetComponent<Image>();
        timer = blinkInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            image.enabled = !image.enabled;
            timer = blinkInterval;
        }
    }
}
