using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class AttackState
    {
        public static void Initialize(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
            var otherSData = f.Unsafe.GetPointer<SurvivorData>(otherSurvivor);
            
            var facing = (otherSData->Position - sData->Position).Normalized;
            sData->Facing = facing;
        }
        
        public static void Update(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);

            if (sData->StateFrame >= f.Global->AttackData.TotalFrames)
            {
                sData->IsStateDone = true;
            }
            else
            {
                sData->StateFrame++;
            }
        }

        public static bool IsActive(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            return sData->StateFrame >= f.Global->AttackData.StartupFrames && 
                   sData->StateFrame < f.Global->AttackData.StartupFrames + f.Global->AttackData.ActiveFrames;
        }
    }
}
