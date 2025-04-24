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
            
            var otherSurvivor = sData->SurvivorID == 1 ? f.Global->Survivor2 : f.Global->Survivor1;
            var otherSData = f.Unsafe.GetPointer<SurvivorData>(otherSurvivor);

            var isActive = sData->StateFrame >= f.Global->AttackData.StartupFrames && 
                           sData->StateFrame < f.Global->AttackData.StartupFrames + f.Global->AttackData.ActiveFrames;

            if (isActive)
            {
                if (IsInRange(f, sData, otherSData))
                {
                    Debug.Log("Is in attack range");
                    SurvivorManager.NotifyAttacked(f, otherSurvivor);
                }
                else
                {
                    Debug.Log("Not in range");
                }
            }

            if (sData->StateFrame >= f.Global->AttackData.TotalFrames)
            {
                sData->IsStateDone = true;
            }
            else
            {
                sData->StateFrame++;
            }
        }

        private static bool IsInRange(Frame f, SurvivorData* survivor1, SurvivorData* survivor2)
        {
            var pos1 = survivor1->Position;
            var pos2 = survivor2->Position;
            var diff = pos1 - pos2;
            return diff.Magnitude < f.Global->AttackData.Range;
        }
    }
}
