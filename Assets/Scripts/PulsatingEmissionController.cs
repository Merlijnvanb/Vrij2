using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HeartbeatEmissionController : MonoBehaviour
{
    [Header("Emission Settings")]
    public Color emissionColor = Color.cyan;
    public float frequency = 2f;        // How fast it pulses (heartbeat speed)
    public float amplitude = 1f;        // Pulse strength
    public float baseEmission = 0.5f;   // Minimum emission level
    public float pulseSpeed = 1f;       // How sharp/smooth the pulse curve is

    private Material _material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        _material = renderer.material;
    }

    void Update()
    {
        if (_material != null)
        {
            _material.SetColor("_EmissionColor", emissionColor);
            _material.SetFloat("_Frequency", frequency);
            _material.SetFloat("_Amplitude", amplitude);
            _material.SetFloat("_BaseEmission", baseEmission);
            _material.SetFloat("_PulseSpeed", pulseSpeed);
        }
    }
}
