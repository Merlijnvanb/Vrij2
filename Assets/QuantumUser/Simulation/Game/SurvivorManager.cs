using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class SurvivorManager
    {
        public static void UpdateSurvivor(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            StateManager.Update(f, entityRef);

            f.Events.UpdateHealth(
                sData->SurvivorID,
                f.Global->BaseHealth,
                sData->Health,
                sData->CurrentState,
                sData->Break,
                sData->Dead);
            //Debug.Log("CurrentState of survivor " + sData->SurvivorID + " : " + sData->CurrentState);
        }

        public static void LockAction(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            StateManager.InitializeState(f, entityRef);
            Log.Debug("Survivor: " + sData->SurvivorID + " Current state: " + sData->CurrentState);
        }

        public static void NotifyAttacked(Frame f, EntityRef entityRef, bool tentacle = false)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.STUN)
                return;

            if (sData->Break)
            {
                if (tentacle) ReceivedTentacleAttack(f, entityRef, true);
                else ReceivedAttack(f, entityRef, true);
                
                Debug.Log(entityRef + " Attack received, Caused break: " + sData->Break + ", Dead: " + sData->Dead + ", Health: " + sData->Health);
                return;
            }
            
            if (tentacle) ReceivedTentacleAttack(f, entityRef);
            else ReceivedAttack(f, entityRef);
            Debug.Log(entityRef + " Attack received, Caused break: " + sData->Break + ", Dead: " + sData->Dead + ", Health: " + sData->Health);
        }
        
        private static void ReceivedAttack(Frame f, EntityRef entityRef, bool breaking = false)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.PARRY && ParryState.IsActive(f, entityRef))
            {
                f.Events.Parry(sData->SurvivorID, sData->Position);
                
                var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
                NotifyAttacked(f, otherSurvivor);
                ParryState.OnSuccess(f, entityRef);
                return;
            }

            if (breaking)
            {
                f.Events.Hit(sData->SurvivorID, sData->Position);
                
                sData->Dead = true;
                StateManager.SetState(f, entityRef, StateID.STUN);
                StunState.Initialize(f, entityRef);
                return;
            }
            
            sData->Health -= f.Global->AttackData.Damage;

            if (sData->Health <= 0)
            {
                sData->Break = true;
            }
            
            f.Events.Hit(sData->SurvivorID, sData->Position);
            
            StateManager.SetState(f, entityRef, StateID.STUN);
            StunState.Initialize(f, entityRef);
        }
        
        private static void ReceivedTentacleAttack(Frame f, EntityRef entityRef, bool breaking = false)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.PARRY && ParryState.IsActive(f, entityRef))
            {
                ParryState.OnSuccess(f, entityRef);
                return;
            }

            if (breaking)
            {
                sData->Dead = true;
                StateManager.SetState(f, entityRef, StateID.STUN);
                StunState.Initialize(f, entityRef, true);
                return;
            }
            
            sData->Health -= f.Global->AttackData.Damage;

            if (sData->Health <= 0)
            {
                sData->Break = true;
            }
            
            StateManager.SetState(f, entityRef, StateID.STUN);
            StunState.Initialize(f, entityRef, true);
        }

        public static void RoundReset(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            sData->Position = sData->SurvivorID == 1 ? new FPVector2(-3, 0) : new FPVector2(3, 0);
            sData->Facing = sData->SurvivorID == 1 ? new FPVector2(1, 0) : new FPVector2(-1, 0);
            sData->Velocity = new FPVector2(0, 0);
            sData->Health = f.Global->BaseHealth;
            sData->Break = false;
            sData->Dead = false;
            sData->CurrentState = StateID.IDLE;
            sData->StateFrame = 0;
            sData->IsStateDone = false;
            sData->AttackHasHit = false;
        }
    }
}
