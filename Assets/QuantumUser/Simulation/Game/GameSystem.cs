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
            f.Global->RoundNumber = 1;
            f.Global->BeatsMaxInterval = FPMath.FloorToInt(gameConfig.BeatIntervalMaxMinSeconds.X * 60);
            f.Global->BeatsMinInterval = FPMath.FloorToInt(gameConfig.BeatIntervalMaxMinSeconds.Y * 60);
            f.Global->PreRoundTimer = FPMath.FloorToInt(gameConfig.PreRoundTimerSeconds * 60);
            f.Global->MaxIntensityTimer = FPMath.FloorToInt(gameConfig.MaxIntensityTimerSeconds * 60);
            f.Global->PostRoundTimer = FPMath.FloorToInt(gameConfig.PostRoundTimerSeconds * 60);
            f.Global->RoundDone = false;
            f.Global->FrictionCoefficient = gameConfig.FrictionCoefficient;
            f.Global->ArenaRadius = gameConfig.ArenaRadius;
            f.Global->CenterRadius = gameConfig.CenterRadius;
            f.Global->TentaclePos = gameConfig.TentacleStart;
            f.Global->TentacleRadius = gameConfig.TentacleRadius;
            f.Global->TentacleActiveBeatsMin = gameConfig.TentacleBeatsActiveMin;
            f.Global->TentacleActiveBeatsMax = gameConfig.TentacleBeatsActiveMax;
            f.Global->CurrentTentacleActiveBeats = 0;
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
            if (!CheckPlayersConnected(f))
                return;
            
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
                if (CheckIfNewRound(f)) ResetRound(f);
                else EndGame(f);
            }
            
            RunBeatTimer(f);

            if (HitReg.TentacleInRange(f, f.Global->Survivor1))
                SurvivorManager.NotifyAttacked(f, f.Global->Survivor1, true);
            
            if (HitReg.TentacleInRange(f, f.Global->Survivor2))
                SurvivorManager.NotifyAttacked(f, f.Global->Survivor2, true);

            if (HitReg.AreInRange(f))
            {
                if (HitReg.CheckSurvivorHit(f))
                {
                    RampBeatIntervalPercentage(f, 5);
                    f.Events.Intensity(f.Global->BeatsRampPercentage);
                }
            }
            
            CollisionHandler.HandleBounds(f);

            if (f.Global->BeatsRampPercentage > 99)
            {
                f.Global->MaxIntensityTimer--;

                if (f.Global->MaxIntensityTimer <= 0)
                {
                    KillBothNoScore(f);
                }
            }
        }

        private bool CheckPlayersConnected(Frame f)
        {
            var sData1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var sData2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);
            //Debug.Log("survivor1 connected: " + sData1->PlayerConnected + ", survivor2 connected: " + sData2->PlayerConnected);
            
            return sData1->PlayerConnected && sData2->PlayerConnected;
        }

        private void KillBothNoScore(Frame f)
        {
            var sData1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var sData2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);
            
            sData1->Dead = true;
            sData2->Dead = true;
        }

        private void CheckDead(Frame f)
        {
            var sData1 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor1);
            var sData2 = f.Unsafe.GetPointer<SurvivorData>(f.Global->Survivor2);

            var isDead1 = sData1->Dead;
            var isDead2 = sData2->Dead;

            if (isDead1 && isDead2)
            {
                f.Global->RoundDone = true;
                return;
            }
            
            if (isDead1) f.Global->S2Score++;
            if (isDead2) f.Global->S1Score++;

            f.Global->RoundDone = isDead1 || isDead2;
        }

        private bool CheckIfNewRound(Frame f)
        {
            if (f.Global->RoundNumber == 3) 
                return false;
            
            return true;
        }

        private void ResetRound(Frame f)
        {
            
            var gameConfig = f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig);
            f.Global->RoundNumber++;
            f.Global->PreRoundTimer = FPMath.FloorToInt(gameConfig.PreRoundTimerSeconds * 60);
            f.Global->MaxIntensityTimer = FPMath.FloorToInt(gameConfig.MaxIntensityTimerSeconds * 60);
            f.Global->PostRoundTimer = FPMath.FloorToInt(gameConfig.PostRoundTimerSeconds * 60);
            f.Global->RoundDone = false;
            
            f.Events.NewRound(f.Global->RoundNumber);
            
            SurvivorManager.RoundReset(f, f.Global->Survivor1);
            SurvivorManager.RoundReset(f, f.Global->Survivor2);
        }

        private void EndGame(Frame f)
        {
            var s1MadeIt = f.Global->S1Score >= 2;
            var s2MadeIt = f.Global->S2Score >= 2;
            
            f.Events.EndGame(s1MadeIt, s2MadeIt);
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
            if (f.Global->CurrentTentacleActiveBeats >= 0)
            {
                f.Global->CurrentTentacleActiveBeats--;
            }
            else
            {
                f.Global->TentaclePos = TentacleManager.GetRandomPoint(f);
                f.Events.TentacleMove(f.Global->TentaclePos);

                var min = f.Global->TentacleActiveBeatsMin;
                var max = f.Global->TentacleActiveBeatsMax;
                f.Global->CurrentTentacleActiveBeats = f.RNG->Next(min, max);
            }
            
            SurvivorManager.LockAction(f, f.Global->Survivor1);
            SurvivorManager.LockAction(f, f.Global->Survivor2);
            f.Events.Heartbeat(f.Global->BeatsRampPercentage);
        }
    }
}
