using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameSystem : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            var gameConfig = f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig);
            f.Global->BeatsMaxInterval = (int)(gameConfig.BeatIntervalMaxMinSeconds.X * 60);
            f.Global->BeatsMinInterval = (int)(gameConfig.BeatIntervalMaxMinSeconds.Y * 60);
            f.Global->FrictionCoefficient = gameConfig.FrictionCoefficient;
            f.Global->ArenaRadius = gameConfig.ArenaRadius;
            f.Global->CenterRadius = gameConfig.CenterRadius;
            f.Global->MoveData = gameConfig.MoveData;
            f.Global->AttackData = gameConfig.AttackData;
            f.Global->ParryData = gameConfig.ParryData;
            f.Global->StunData = gameConfig.StunData;
            f.Global->BeatsRampPercentage = 0;
            f.Global->BeatsTimer = 0;
        }
        
        public override void Update(Frame f)
        {
            RunBeatTimer(f);
            
            // should probably handle hit checking from here, right now survivor 1 always gets priority I think
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor1);
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor2);

            if (HitReg.AreInRange(f))
            {
                if (HitReg.CheckHit(f)) RampBeatIntervalPercentage(f, 20);
            }
            
            CollisionHandler.HandleBounds(f);
        }

        private void RampBeatIntervalPercentage(Frame f, int amount)
        {
            var currentPercent = f.Global->BeatsRampPercentage;
            var newPercent = currentPercent + amount;
            
            f.Global->BeatsRampPercentage = FPMath.Clamp(newPercent, 0, 100);
        }

        private void RunBeatTimer(Frame f)
        {
            var currentInterval = CalculateInterval(f);

            if (f.Global->BeatsTimer <= 0)
            {
                OnBeat(f);
                Log.Debug("Heartbeat, currentInterval is now: " + currentInterval);
                f.Global->BeatsTimer = currentInterval;
            }
            else
            {
                f.Global->BeatsTimer--;
            }
        }

        private int CalculateInterval(Frame f)
        {
            var interval = FPMath.Lerp(f.Global->BeatsMaxInterval, f.Global->BeatsMinInterval, f.Global->BeatsRampPercentage / 100);
            return (int)interval;
        }

        private void OnBeat(Frame f)
        {
            SurvivorManager.LockAction(f, f.Global->Survivor1);
            SurvivorManager.LockAction(f, f.Global->Survivor2);
            f.Events.Heartbeat(f.Global->BeatsRampPercentage);
        }
    }
}
