namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        public FPVector2 BeatIntervalMaxMinSeconds;
        public FP FrictionCoefficient;
        public FP ArenaRadius;
        public FP CenterRadius;
        
        public MoveData MoveData;
        public AttackData AttackData;
        public ParryData ParryData;
        public StunData StunData;
    }
}
