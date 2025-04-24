namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        public FPVector2 BeatIntervalMaxMinSeconds;
        public FP FrictionCoefficient;
        
        public MoveData MoveData;
        public AttackData AttackData;
        public ParryData ParryData;
        public StunData StunData;
    }
}
