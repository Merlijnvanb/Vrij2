namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StateManager
    {
        public static void Update(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->IsStateDone)
            {
                sData->CurrentState = StateID.IDLE;
            }

            switch (sData->CurrentState)
            {
                case StateID.MOVE:
                    MoveState.Update(f, entityRef);
                    break;
                
                case StateID.ATTACK:
                    AttackState.Update(f, entityRef);
                    break;
                
                case StateID.PARRY:
                    ParryState.Update(f, entityRef);
                    break;
                
                case StateID.STUN:
                    StunState.Update(f, entityRef);
                    break;
            }
        }
        
        public static void InitializeState(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.STUN)
                return;
            
            var state = WhichState(f, entityRef);
            sData->CurrentState = state;
            sData->StateFrame = 0;
            sData->IsStateDone = false;
            
            switch (state)
            {
                case StateID.MOVE:
                    MoveState.Initialize(f, entityRef);
                    break;
                
                case StateID.ATTACK:
                    AttackState.Initialize(f, entityRef);
                    break;
                
                case StateID.PARRY:
                    ParryState.Initialize(f, entityRef);
                    break;
            }
        }

        public static void ReceivedAttack(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->CurrentState == StateID.PARRY && ParryState.IsActive(f, entityRef))
            {
                var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
                SurvivorManager.NotifyAttacked(f, otherSurvivor);
                ParryState.OnSuccess(f, entityRef);
                return;
            }
            
            sData->CurrentState = StateID.STUN;
            sData->StateFrame = 0;
            sData->IsStateDone = false;
            
            StunState.Initialize(f, entityRef);
        }

        private static StateID WhichState(Frame f, EntityRef entityRef)
        {
            var dirVector = InputHandler.GetDirectionVector(f, entityRef);
            
            if (dirVector != FPVector2.Zero)
            {
                return StateID.MOVE;
            }
            
            return InputHandler.GetPriorityState(f, entityRef);
        }
    }
}
