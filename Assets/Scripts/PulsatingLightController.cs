using UnityEngine;

[RequireComponent(typeof(Light))]
public class PulsatingLightController : MonoBehaviour
{
    [Header("Pulsation Settings")]
    public float frequency = 2f;           // Speed of the pulse (heartbeat-like)
    public float amplitude = 1f;           // How strong the pulse is
    public float baseIntensity = 0.5f;     // Minimum light intensity
    public float pulseSpeed = 1f;          // Sharpness of the pulse curve

    [Header("Material Settings")]
    public Material material;              // The material to apply pulsation to
    public Color emissionColor = Color.cyan; // Emission color

    private Light pointLight;
    private float time;

    void Start()
    {
        // Get the Light component attached to this object
        pointLight = GetComponent<Light>();

        // Set the light's initial intensity
        pointLight.intensity = baseIntensity;

        // Set the material's emission color
        if (material != null)
        {
            material.SetColor("_EmissionColor", emissionColor);
        }
    }

    void Update()
    {
        // Update time to drive the pulsation
        time += Time.deltaTime;

        // Create a heartbeat-like sine wave pulse
        float pulseValue = Mathf.Abs(Mathf.Sin(time * frequency));  // Smooth sine pulse between 0 and 1
        pulseValue = Mathf.Pow(pulseValue, pulseSpeed);               // Make pulse sharper or smoother

        // Set the light intensity to pulse
        pointLight.intensity = baseIntensity + (pulseValue * amplitude);

        // Sync the material emission with the light intensity
        if (material != null)
        {
            material.SetColor("_EmissionColor", emissionColor * (pulseValue * amplitude));
        }
    }
}
