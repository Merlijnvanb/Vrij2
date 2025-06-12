namespace Quantum
{
    using Quantum;
    using UnityEngine;
    using Unity.Cinemachine;
    using System.Collections.Generic;

    public class SurvivorView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        [System.Serializable]
        public struct AnimationProperties
        {
            public List<Sprite> Sprites;
            public bool Loop;
            public float SingleSpriteDuration;
        }
        
        [System.Serializable]
        public struct SurvivorAnimations
        {
            public AnimationProperties Idle;
            public AnimationProperties Move;
            public AnimationProperties Attack;
            public AnimationProperties Parry;
            public AnimationProperties Stun;
            public AnimationProperties Death;
        }
        
        public Transform Body;
        public CinemachineTargetGroup TargetGroup;
        public SpriteRenderer SpriteRenderer;
        public SpriteRenderer HealthSpriteRenderer;
        public SurvivorAnimations Animations;

        public Material Survivor1HealthMat;
        public Material Survivor2HealthMat;

        private bool groupAssigned;
        private Sprite currentSprite;
        private StateID currentState;
        private AnimationProperties currentAnim;
        private float spriteTime;
        private int currentSpriteIndex;
        private Vector3 startingScale;

        void Start()
        {
            QuantumEvent.Subscribe<EventUpdateHealth>(listener: this, handler: UpdateHealth);
            
            TargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
            groupAssigned = true;
            startingScale = new Vector3(1.5f, 1.5f, 1.5f);
            
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;
            
            HealthSpriteRenderer.material = survivorData.SurvivorID == 1 ? Survivor1HealthMat : Survivor2HealthMat;
            currentState = survivorData.CurrentState;
            currentAnim = GetAnimation(currentState);
            spriteTime = 0;
            currentSpriteIndex = 0;
            
            Debug.Log("Survivor View setup successfully");
        }

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;

            if (survivorData.CurrentState != currentState)
            {
                currentState = survivorData.CurrentState;
                currentAnim = GetAnimation(currentState);
                spriteTime = 0;
                currentSpriteIndex = 0;
            }

            if (spriteTime < currentAnim.SingleSpriteDuration)
            {
                spriteTime += Time.deltaTime;
            }
            else
            {
                spriteTime = 0;
                UpdateSprite();
            }
            
            var pos = survivorData.Position.ToUnityVector2();
            var facing = survivorData.Facing.ToUnityVector2();
            var facingLeft = facing.x < 0;
            var facingRight = facing.x > 0;
            
            Body.position = new Vector3(pos.x, Body.position.y, pos.y);
            
            if (facingRight)
                Body.localScale = startingScale;
            
            if (facingLeft)
                Body.localScale = new Vector3(-startingScale.x, startingScale.y, startingScale.z);

            if (currentSprite != null)
            {
                SpriteRenderer.sprite = currentSprite;
                HealthSpriteRenderer.sprite = currentSprite;
            }
            
            if (!groupAssigned)
                return;
            
            if (survivorData.SurvivorID == 1)
                TargetGroup.Targets[0].Object = Body;
            else
                TargetGroup.Targets[1].Object = Body;
        }

        private void UpdateHealth(EventUpdateHealth e)
        {
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;

            if (survivorData.SurvivorID != e.SurvivorID)
                return;
            
            if (e.CurrentState == StateID.STUN || e.Break)
                HealthSpriteRenderer.material.SetInt("_Flicker", 1);
            else
                HealthSpriteRenderer.material.SetInt("_Flicker", 0);

            if (!e.Break)
            {
                var healthNormalized = e.CurrentHealth / e.MaxHealth;
                HealthSpriteRenderer.material.SetFloat("_HealthNormalized", healthNormalized.AsFloat);
            }
            else if (e.Break && !e.Dead)
            {
                var healthNormalized = 1f;
                HealthSpriteRenderer.material.SetFloat("_HealthNormalized", healthNormalized);
            }
            else if (e.Dead)
            {
                var healthNormalized = 0f;
                HealthSpriteRenderer.material.SetFloat("_HealthNormalized", healthNormalized);
            }
        }

        private void UpdateSprite()
        {
            if (currentAnim.Sprites == null)
                return;
            
            if (currentSpriteIndex < currentAnim.Sprites.Count - 1)
            {
                currentSpriteIndex++;
            }
            else if (currentAnim.Loop)
            {
                currentSpriteIndex = 0;
            }
            
            currentSprite = currentAnim.Sprites[currentSpriteIndex];
        }

        private AnimationProperties GetAnimation(StateID state)
        {
            switch (state)
            {
                case StateID.IDLE:
                    return Animations.Idle;
                
                case StateID.MOVE:
                    return Animations.Move;
                
                case StateID.ATTACK:
                    return Animations.Attack;
                
                case StateID.PARRY:
                    return Animations.Parry;
                
                case StateID.STUN:
                    return Animations.Stun;
                
                case StateID.DEATH:
                    return Animations.Death;
            }

            return Animations.Idle;
        }
    }
}
