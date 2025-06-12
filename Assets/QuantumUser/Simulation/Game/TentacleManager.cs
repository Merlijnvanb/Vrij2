namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class TentacleManager
    {
        public static FPVector2 GetRandomPoint(Frame f)
        {
            var minRadius = f.Global->CenterRadius + f.Global->TentacleRadius;
            var maxRadius = f.Global->ArenaRadius - f.Global->TentacleRadius;

            var angle = f.RNG->Next(FP._0, FP.Pi * FP._2);
            
            var minSq = minRadius * minRadius;
            var maxSq = maxRadius * maxRadius;
            var distance = FPMath.Sqrt(f.RNG->Next(minSq, maxSq));
            
            var x = FPMath.Cos(angle) * distance;
            var y = FPMath.Sin(angle) * distance;
            
            return new FPVector2(x, y);
        }
    }
}
