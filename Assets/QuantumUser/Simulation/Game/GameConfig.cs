namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject
    {
        public FPVector2 BeatIntervalMaxMinSeconds;
        
        public FP PreRoundTimerSeconds;
        public FP MaxIntensityTimerSeconds;
        public FP PostRoundTimerSeconds;
        
        public FP FrictionCoefficient;
        public FP ArenaRadius;
        public FP CenterRadius;

        public FP BaseHealth;
        
        public MoveData MoveData;
        public AttackData AttackData;
        public ParryData ParryData;
        public StunData StunData;
    }
}
