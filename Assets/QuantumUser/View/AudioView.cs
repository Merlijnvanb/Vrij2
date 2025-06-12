namespace Quantum
{
    using UnityEngine;
    using FMODUnity;

    public class AudioView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        [System.Serializable]
        public struct AudioSource
        {
            public Transform Transform;
            public StudioEventEmitter Emitter;
        }
        
        [System.Serializable]
        public struct SurvivorAudio
        {
            public AudioSource Footstep;
            public AudioSource Hit;
            public AudioSource Parry;
            public AudioSource Death;
        }
        
        public StudioEventEmitter Ambience;
        public StudioEventEmitter Camera;
        public SurvivorAudio Survivor1;
        public SurvivorAudio Survivor2;

        void Start()
        {
            QuantumEvent.Subscribe<EventFootstep>(listener: this, handler: HandleFootstep);
            QuantumEvent.Subscribe<EventHit>(listener: this, handler: HandleHit);
            QuantumEvent.Subscribe<EventParry>(listener: this, handler: HandleParry);
            QuantumEvent.Subscribe<EventDeath>(listener: this, handler: HandleDeath);
        }

        private void HandleFootstep(EventFootstep e)
        {
            var sAudio = e.SurvivorID == 1 ? Survivor1 : Survivor2;
            var pos = e.Pos.ToUnityVector2();
            
            sAudio.Footstep.Transform.position = new Vector3(pos.x, 0, pos.y);
            sAudio.Footstep.Emitter.Play();
        }

        private void HandleHit(EventHit e)
        {
            var sAudio = e.SurvivorID == 1 ? Survivor1 : Survivor2;
            var pos = e.Pos.ToUnityVector2();
            
            sAudio.Hit.Transform.position = new Vector3(pos.x, 0, pos.y);
            sAudio.Hit.Emitter.Play();
        }

        private void HandleParry(EventParry e)
        {
            var sAudio = e.SurvivorID == 1 ? Survivor1 : Survivor2;
            var pos = e.Pos.ToUnityVector2();
            
            sAudio.Parry.Transform.position = new Vector3(pos.x, 0, pos.y);
            sAudio.Parry.Emitter.Play();
        }

        private void HandleDeath(EventDeath e)
        {
            var sAudio = e.SurvivorID == 1 ? Survivor1 : Survivor2;
            var pos = e.Pos.ToUnityVector2();
            
            sAudio.Death.Transform.position = new Vector3(pos.x, 0, pos.y);
            sAudio.Death.Emitter.Play();
        }
    }
}
