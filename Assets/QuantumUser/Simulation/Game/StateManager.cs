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
                sData->IsStateDone = false;
                sData->CurrentState = StateID.IDLE;
            }

            switch (sData->CurrentState)
            {
                case StateID.MOVE:
                    MovingState.Update(f, entityRef);
                    break;
            }
        }
        
        public static void InitializeState(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            var state = WhichState(f, entityRef);
            sData->CurrentState = state;
            
            switch (state)
            {
                case StateID.MOVE:
                    MovingState.Initialize(f, entityRef);
                    break;
                
                case StateID.ATTACK:
                    
                    break;
                
                case StateID.PARRY:
                    
                    break;
            }
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
