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
        public SurvivorAnimations Animations;

        private bool groupAssigned;
        private Sprite currentSprite;
        private StateID currentState;
        private AnimationProperties currentAnim;
        private float spriteTime;
        private int currentSpriteIndex;

        void Start()
        {
            TargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
            groupAssigned = true;
            
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;
            
            currentState = survivorData.CurrentState;
            currentAnim = GetAnimation(currentState);
            spriteTime = 0;
            currentSpriteIndex = 0;
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
            var facing3D = facing.x > 0 ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            
            Body.position = new Vector3(pos.x, Body.position.y, pos.y);
            Body.rotation = Quaternion.LookRotation(facing3D, Vector3.up);
            
            if (currentSprite != null)
                SpriteRenderer.sprite = currentSprite;
            
            if (!groupAssigned)
                return;
            
            if (survivorData.SurvivorID == 1)
                TargetGroup.Targets[0].Object = Body;
            else
                TargetGroup.Targets[1].Object = Body;
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
