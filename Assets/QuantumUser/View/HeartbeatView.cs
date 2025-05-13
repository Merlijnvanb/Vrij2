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
        public float EmissionIntensity;
        public float FadeInDuration;
        public float FadeOutDuration;
        public float OnDuration;

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
            StartCoroutine(HeartLighting());
            
            var light1 = LeanTween.value(Spotlight.gameObject, 0, LightIntensity, FadeInDuration);
            var heart1 = LeanTween.value(HeartGO, emissionBase, EmissionIntensity,FadeInDuration);

            light1.setOnUpdate((f) => Spotlight.intensity = f);
            heart1.setOnUpdate((f) => heartMaterial.SetFloat("_EmissionMultiplier", f));
            light1.setEaseInCubic();
            heart1.setEaseInCubic();
            
            AudioEmitter.Play();
        }

        private IEnumerator HeartLighting()
        {
            yield return new WaitForSeconds(FadeInDuration + OnDuration);
            
            var light2 = LeanTween.value(Spotlight.gameObject, LightIntensity, 0, FadeOutDuration);
            var heart2 = LeanTween.value(HeartGO, EmissionIntensity, emissionBase,FadeOutDuration);

            light2.setOnUpdate((f) => Spotlight.intensity = f);
            heart2.setOnUpdate((f) => heartMaterial.SetFloat("_EmissionMultiplier", f));
            light2.setEaseInCubic();
            heart2.setEaseInCubic();
        }
    }
}
