namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class ParryState
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
            
            if (sData->StateFrame >= f.Global->ParryData.TotalFrames)
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
            
            var isActive = sData->StateFrame >= f.Global->ParryData.StartupFrames && 
                           sData->StateFrame < f.Global->ParryData.StartupFrames + f.Global->ParryData.ActiveFrames;
            
            return isActive;
        }

        public static void OnSuccess(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            sData->IsStateDone = true;
        }
    }
}
