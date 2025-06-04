namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class CollisionHandler
    {
        public static void HandleBounds(Frame f)
        {
            OuterBounds(f, f.Global->Survivor1);
            OuterBounds(f, f.Global->Survivor2);
            
            InnerBounds(f, f.Global->Survivor1);
            InnerBounds(f, f.Global->Survivor2);
        }

        private static void OuterBounds(Frame f, EntityRef entity)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entity);
            
            var pos = sData->Position;
            var center = new FPVector2(0, 0);
            var distance = FPVector2.Distance(pos, center);
            var radius = f.Global->ArenaRadius;

            if (distance > radius)
            {
                var outsideDistance = distance - radius;
                var newPos = FPVector2.MoveTowards(pos, center, outsideDistance);
                
                sData->Position = newPos;
            }
        }

        private static void InnerBounds(Frame f, EntityRef entity)
        {
            var sData = f.Unsafe.GetPointer<SurvivorData>(entity);
            
            var pos = sData->Position;
            var center = new FPVector2(0, 0);
            var distance = FPVector2.Distance(pos, center);
            var radius = f.Global->CenterRadius;

            if (distance < radius)
            {
                var insideDistance = radius - distance;
                var newPos = FPVector2.MoveTowards(pos, center, -insideDistance);
                
                sData->Position = newPos;
            }
        }
    }
}
