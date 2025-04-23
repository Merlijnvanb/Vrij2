namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        public FPVector2 BeatIntervalMaxMinSeconds;
        public FP MoveVelocity = 5;
        public FP FrictionCoefficient = FP._0_50;
    }
}
