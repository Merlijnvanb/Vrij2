namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InputHandler
    {
        public static FPVector2 GetDirectionVector(Frame f, EntityRef entityRef)
        {
            if (!f.Unsafe.TryGetPointer(entityRef, out PlayerLink* playerLink))
                return FPVector2.Zero;
         
            var currentInput = f.GetPlayerInput(playerLink->PlayerRef);
            FPVector2 vector = FPVector2.Zero;
            
            if (currentInput->Left) vector.X -= 1;
            if (currentInput->Right) vector.X += 1;
            
            if (currentInput->Up) vector.Y += 1;
            if (currentInput->Down) vector.Y -= 1;

            return vector.Normalized;
        }

        public static StateID GetPriorityState(Frame f, EntityRef entityRef)
        {
            if (!f.Unsafe.TryGetPointer(entityRef, out PlayerLink* playerLink))
                return StateID.IDLE;
         
            var currentInput = f.GetPlayerInput(playerLink->PlayerRef);

            if (currentInput->Block) return StateID.BLOCK;
            if (currentInput->Attack) return StateID.ATTACK;
            if (currentInput->Parry) return StateID.PARRY;
            return StateID.IDLE;
        }
    }
}
