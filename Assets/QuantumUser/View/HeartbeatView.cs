using System.Collections;
using Unity.VisualScripting;
using FMODUnity;

namespace Quantum
{
    using UnityEngine;

    public class HeartbeatView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        
        [Header("VISUALS")]
        public Light[] Lights;
        //public GameObject HeartGO;
        

        [Header("Parameters")] 
        public float LightIntensity;
        //public float EmissionIntensity;
        public float FadeInDuration;
        public float OnDuration;
        public float FallOffFactor;

        [Header("AUDIO")] 
        public StudioEventEmitter AudioEmitter;
        
        //private Material heartMaterial;
        //private float emissionBase;
        private float lightBase;
        private float factor = 0;
        //private bool fallOff = false;
        
        void Start()
        {
            QuantumEvent.Subscribe<EventHeartbeat>(listener: this, handler: HandleBeat);
            
            //heartMaterial = HeartGO.GetComponent<Renderer>().material;
            //emissionBase = heartMaterial.GetFloat("_EmissionMultiplier");
            lightBase = 0;
        }

        void Update()
        {
            if (factor > 0)
                factor -= FallOffFactor * Time.deltaTime;
            else
                factor = 0;
            
            var lightValue = Mathf.Lerp(lightBase, LightIntensity, factor);
            //var emissionValue = Mathf.Lerp(emissionBase, EmissionIntensity, factor);

            foreach (var light in Lights)
            {
                light.intensity = lightValue;
            }
            //heartMaterial.SetFloat("_EmissionMultiplier", emissionValue);
        }

        private void HandleBeat(EventHeartbeat e)
        {
            StartCoroutine(LightingSequence());
            
            // var light1 = LeanTween.value(Spotlight.gameObject, 0, LightIntensity, FadeInDuration);
            // var heart1 = LeanTween.value(HeartGO, emissionBase, EmissionIntensity,FadeInDuration);
            //
            // light1.setOnUpdate((f) => Spotlight.intensity = f);
            // heart1.setOnUpdate((f) => heartMaterial.SetFloat("_EmissionMultiplier", f));
            // light1.setEaseInCubic();
            // heart1.setEaseInCubic();
            
            AudioEmitter.Play();
        }

        private IEnumerator LightingSequence()
        {
            LTDescr factorTween;
            factorTween = LeanTween.value(factor, 1f, FadeInDuration);
            factorTween.setOnUpdate((f) => factor = f);
            
            yield return new WaitForSeconds(FadeInDuration);
            yield return new WaitForSeconds(OnDuration);
          
            factorTween = LeanTween.value(factor, 1f, FadeInDuration);
            factorTween.setOnUpdate((f) => factor = f);
            
            // var light2 = LeanTween.value(Spotlight.gameObject, LightIntensity, 0, FadeOutDuration);
            // var heart2 = LeanTween.value(HeartGO, EmissionIntensity, emissionBase,FadeOutDuration);
            //
            // light2.setOnUpdate((f) => Spotlight.intensity = f);
            // heart2.setOnUpdate((f) => heartMaterial.SetFloat("_EmissionMultiplier", f));
            // light2.setEaseInCubic();
            // heart2.setEaseInCubic();
        }
    }
}
