using System.Collections;
using Unity.VisualScripting;
using FMODUnity;

namespace Quantum
{
    using UnityEngine;

    public class HeartbeatView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        
        [Header("VISUALS")]
        public Light Spotlight;
        public GameObject HeartGO;
        

        [Header("Parameters")] 
        public float LightIntensity;
        public float EmissionTo;
        public float LightDuration;
        public float HeartDuration;

        [Header("AUDIO")] 
        public StudioEventEmitter AudioEmitter;
        
        private Material heartMaterial;
        private float emissionBase;
        
        void Start()
        {
            QuantumEvent.Subscribe<EventHeartbeat>(listener: this, handler: HandleBeat);
            
            heartMaterial = HeartGO.GetComponent<Renderer>().material;
            emissionBase = heartMaterial.GetFloat("_EmissionMultiplier");
        }

        private void HandleBeat(EventHeartbeat e)
        {
            LeanTween.value(Spotlight.gameObject, LightIntensity, 0, LightDuration).setOnUpdate((f) => Spotlight.intensity = f);
            LeanTween.value(HeartGO, EmissionTo, emissionBase,HeartDuration).setOnUpdate((f) => heartMaterial.SetFloat("_EmissionMultiplier", f));
            
            AudioEmitter.Play();
        }
    }
}
