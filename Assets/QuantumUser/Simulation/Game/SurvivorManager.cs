namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class SurvivorManager
    {
        // public struct Filter
        // {
        //     public EntityRef Entity;
        //     public SurvivorData* SurvivorData;
        // }

        public static void UpdateSurvivor(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            StateManager.Update(f, entityRef);
        }

        public static void LockAction(Frame f, EntityRef entityRef)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entityRef);
            
            StateManager.InitializeState(f, entityRef);
            Log.Debug("Survivor: " + sData->SurvivorID + " Current state: " + sData->CurrentState);
        }
    }
}
