using System.Collections;

namespace Quantum
{
    using UnityEngine;

    public class HeartbeatView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Light Spotlight;
        public Material HeartMaterial;

        [Header("Parameters")] 
        public float LightIntensity;
        public float HeartIntensity;
        public float LightDuration;
        public float HeartDuration;
        
        void Start()
        {
            QuantumEvent.Subscribe<EventHeartbeat>(listener: this, handler: HandleBeat);
        }

        private void HandleBeat(EventHeartbeat e)
        {
            LeanTween.value(Spotlight.gameObject, 0, 1, LightDuration).setOnUpdate((f) => Spotlight.intensity = f).setEaseInCubic();
        }
    }
}
