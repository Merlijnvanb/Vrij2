namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class MoveState
    {
        public static void Initialize(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            var directionVector = InputHandler.GetDirectionVector(f, entityRef);
            
            sData->Facing = directionVector;
            sData->Velocity = f.Global->MoveData.MoveVelocity * directionVector;
        }

        public static void Update(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            CalculateVelocity(f, entityRef);
            ApplyVelocity(f, entityRef);

            if (sData->Velocity == FPVector2.Zero)
                sData->IsStateDone = true;
        }

        private static void CalculateVelocity(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            if (sData->Velocity.X != 0) sData->Velocity.X -= f.Global->FrictionCoefficient * sData->Velocity.X;
            if (FPMath.Abs(sData->Velocity.X) < FP._0_01) sData->Velocity.X = 0;
            
            if (sData->Velocity.Y != 0) sData->Velocity.Y -= f.Global->FrictionCoefficient * sData->Velocity.Y;
            if (FPMath.Abs(sData->Velocity.Y) < FP._0_01) sData->Velocity.Y = 0;
        }

        private static void ApplyVelocity(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            sData->Position += sData->Velocity;
        }
    }
}
