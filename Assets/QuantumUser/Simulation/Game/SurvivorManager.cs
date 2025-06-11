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
            Debug.Log("CurrentState of survivor " + sData->SurvivorID + " : " + sData->CurrentState);
        }

        public static void LockAction(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            StateManager.InitializeState(f, entityRef);
            Log.Debug("Survivor: " + sData->SurvivorID + " Current state: " + sData->CurrentState);
        }

        public static void NotifyAttacked(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            Debug.Log(entityRef + " Attack recieved, Break: " + sData->Break + ", Dead: " + sData->Dead + ", Health: " + sData->Health);

            if (sData->Break)
            {
                ReceivedAttack(f, entityRef, true);
                return;
            }
            
            ReceivedAttack(f, entityRef);
        }
        
        private static void ReceivedAttack(Frame f, EntityRef entityRef, bool breaking = false)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.PARRY && ParryState.IsActive(f, entityRef))
            {
                var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
                NotifyAttacked(f, otherSurvivor);
                ParryState.OnSuccess(f, entityRef);
                return;
            }

            if (breaking)
            {
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
            
            StateManager.SetState(f, entityRef, StateID.STUN);
            StunState.Initialize(f, entityRef);
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
