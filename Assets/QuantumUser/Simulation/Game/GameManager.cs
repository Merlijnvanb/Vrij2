using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameManager : SystemMainThread
    {
        public override void OnInit(Frame f)
        {
            var gameConfig = f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig);
            f.Global->BeatsMaxInterval = (int)(gameConfig.BeatIntervalMaxMinSeconds.X * 60);
            f.Global->BeatsMinInterval = (int)(gameConfig.BeatIntervalMaxMinSeconds.Y * 60);
            f.Global->BeatsRampPercentage = 0;
            f.Global->BeatsTimer = 0;
        }
        
        public override void Update(Frame f)
        {
            RunBeatTimer(f);
        }

        private void RunBeatTimer(Frame f)
        {
            var currentInterval = CalculateInterval(f);

            if (f.Global->BeatsTimer <= 0)
            {
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
    }
}
