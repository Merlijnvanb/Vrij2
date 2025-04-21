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

            if (sData->StateDuration <= 0)
            {
                sData->CurrentState = StateID.IDLE;
            }
            else
            {
                sData->StateDuration--;
            }
        }
        
        public static void InitializeState(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            var state = WhichState(f, entityRef, out var directionVector);
            sData->CurrentState = state;
            
            switch (state)
            {
                case StateID.MOVE:
                    sData->Velocity = f.Global->MoveVelocity * directionVector; // make separate scripts for each state with initialize and update functions
                    break;
                
                case StateID.ATTACK:
                    
                    break;
                
                case StateID.PARRY:
                    
                    break;
            }
        }

        private static StateID WhichState(Frame f, EntityRef entityRef, out FPVector2 outDirVector)
        {
            var dirVector = InputHandler.GetDirectionVector(f, entityRef);
            outDirVector = dirVector;
            
            if (dirVector != FPVector2.Zero)
            {
                return StateID.MOVE;
            }
            
            return InputHandler.GetPriorityState(f, entityRef);
        }
    }
}
