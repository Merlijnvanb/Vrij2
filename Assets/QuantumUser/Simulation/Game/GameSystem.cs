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
            f.Global->S1Score = 0;
            f.Global->S2Score = 0;
            f.Global->BeatsMaxInterval = FPMath.FloorToInt(gameConfig.BeatIntervalMaxMinSeconds.X * 60);
            f.Global->BeatsMinInterval = FPMath.FloorToInt(gameConfig.BeatIntervalMaxMinSeconds.Y * 60);
            f.Global->PreRoundTimer = FPMath.FloorToInt(gameConfig.PreRoundTimerSeconds * 60);
            f.Global->MaxIntensityTimer = FPMath.FloorToInt(gameConfig.MaxIntensityTimerSeconds * 60);
            f.Global->PostRoundTimer = FPMath.FloorToInt(gameConfig.PostRoundTimerSeconds * 60);
            f.Global->RoundDone = false;
            f.Global->FrictionCoefficient = gameConfig.FrictionCoefficient;
            f.Global->ArenaRadius = gameConfig.ArenaRadius;
            f.Global->CenterRadius = gameConfig.CenterRadius;
            f.Global->BaseHealth = gameConfig.BaseHealth;
            f.Global->MoveData = gameConfig.MoveData;
            f.Global->AttackData = gameConfig.AttackData;
            f.Global->ParryData = gameConfig.ParryData;
            f.Global->StunData = gameConfig.StunData;
            f.Global->BeatsRampPercentage = 0;
            f.Global->BeatsTimer = 0;
        }
        
        public override void Update(Frame f)
        {
            if (!f.Global->RoundDone) CheckDead(f);
            
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor1);
            SurvivorManager.UpdateSurvivor(f, f.Global->Survivor2);
            
            if (f.Global->PreRoundTimer > 0)
            {
                f.Global->PreRoundTimer--;
                return;
            }

            if (f.Global->RoundDone && f.Global->PostRoundTimer > 0)
            {
                f.Global->PostRoundTimer--;
                return;
            }
            else if (f.Global->RoundDone && f.Global->PostRoundTimer <= 0)
            {
                ResetRound(f);
            }
            
            RunBeatTimer(f);

            if (HitReg.AreInRange(f))
            {
                if (HitReg.CheckHit(f))
                {
                    RampBeatIntervalPercentage(f, 20);
                }
            }
            
            CollisionHandler.HandleBounds(f);

            if (f.Global->BeatsRampPercentage > 99)
            {
                f.Global->MaxIntensityTimer--;

                if (f.Global->MaxIntensityTimer <= 0)
                {
                    CheckBreak(f);
                }
            }
        }

        private void CheckBreak(Frame f)
        {
            var sData1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var sData2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);

            var isBreak1 = sData1->Break;
            var isBreak2 = sData2->Break;

            if (isBreak1)
            {
                sData1->Dead = true;
            }

            if (isBreak2)
            {
                sData2->Dead = true;
            }
        }

        private void CheckDead(Frame f)
        {
            var sData1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var sData2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);

            var isDead1 = sData1->Dead;
            var isDead2 = sData2->Dead;

            if (isDead1) f.Global->S2Score++;
            if (isDead2) f.Global->S1Score++;

            f.Global->RoundDone = isDead1 || isDead2;
        }

        private void ResetRound(Frame f)
        {
            var gameConfig = f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig);
            f.Global->PreRoundTimer = FPMath.FloorToInt(gameConfig.PreRoundTimerSeconds * 60);
            f.Global->MaxIntensityTimer = FPMath.FloorToInt(gameConfig.MaxIntensityTimerSeconds * 60);
            f.Global->PostRoundTimer = FPMath.FloorToInt(gameConfig.PostRoundTimerSeconds * 60);
            f.Global->RoundDone = false;
            
            SurvivorManager.RoundReset(f, f.Global->Survivor1);
            SurvivorManager.RoundReset(f, f.Global->Survivor2);
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
