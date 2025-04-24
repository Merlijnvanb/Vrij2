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
            f.Global->BeatsRampPercentage = 0;
            f.Global->BeatsTimer = 0;
            f.Global->FrictionCoefficient = gameConfig.FrictionCoefficient;
            f.Global->MoveData = gameConfig.MoveData;
            f.Global->AttackData = gameConfig.AttackData;
            f.Global->ParryData = gameConfig.ParryData;
        }
        
        public override void Update(Frame f)
        {
            RunBeatTimer(f);
            
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor1);
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor2);
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
        }
    }
}
