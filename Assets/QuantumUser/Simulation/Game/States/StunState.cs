namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StunState
    {
        public static void Initialize(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
            var otherSData = f.Unsafe.GetPointer<SurvivorData>(otherSurvivor);
            
            var facing = (otherSData->Position - sData->Position).Normalized;
            sData->Facing = facing;
            
            sData->Velocity = -facing * f.Global->AttackData.Knockback;
        }
        
        public static void Update(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            CalculateVelocity(f, entityRef);
            ApplyVelocity(f, entityRef);
            
            if (sData->StateFrame >= f.Global->StunData.Duration)
            {
                sData->IsStateDone = true;
            }
            else
            {
                sData->StateFrame++;
            }
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
