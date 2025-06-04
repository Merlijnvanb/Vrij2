namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class HitReg
    {
        public static bool AreInRange(Frame f)
        {
            var survivor1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var survivor2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);
            
            var pos1 = survivor1->Position;
            var pos2 = survivor2->Position;
            var diff = pos1 - pos2;
            return diff.Magnitude < f.Global->AttackData.Range;
        }

        public static bool CheckHit(Frame f)
        {
            var survivor1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var survivor2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);

            var attackActive1 = survivor1->CurrentState == StateID.ATTACK && AttackState.IsActive(f, f.Global->Survivor1);
            var attackActive2 = survivor2->CurrentState == StateID.ATTACK && AttackState.IsActive(f, f.Global->Survivor2);

            if (attackActive1 && !survivor1->AttackHasHit)
            {
                survivor1->AttackHasHit = true;
                SurvivorManager.NotifyAttacked(f, f.Global->Survivor2);
                return true;
            }

            if (attackActive2 && !survivor2->AttackHasHit)
            {
                survivor2->AttackHasHit = true;
                SurvivorManager.NotifyAttacked(f, f.Global->Survivor1);
                return true;
            }

            return false;
        }
    }
}
