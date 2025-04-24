namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInitializer : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public override void OnInit(Frame f)
        {
            for (int i = 1; i <= 2; i++)
            {
                var survivorEntity = f.Create(f.FindAsset(f.RuntimeConfig.SurvivorPrototype));
                var sData = f.Unsafe.GetPointer<SurvivorData>(survivorEntity);

                sData->SurvivorID = i;
                sData->Position = i == 1 ? new FPVector2(-3, 0) : new FPVector2(3, 0);
                sData->Facing = i == 1 ? new FPVector2(1, 0) : new FPVector2(-1, 0);
                sData->Velocity = new FPVector2(0, 0);
                sData->CurrentState = StateID.IDLE;
                sData->StateFrame = 0;
                sData->IsStateDone = false;
                
                if (i == 1)
                    f.Global->Survivor1 = survivorEntity;
                else
                    f.Global->Survivor2 = survivorEntity;
            }
        }
        
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            if (player._index > 2)
                return;
            
            var survivorEntity = player._index == 1 ? f.Global->Survivor1 : f.Global->Survivor2;
            var playerLink = f.Unsafe.GetPointer<PlayerLink>(survivorEntity);
            
            playerLink->PlayerRef = player;
        }
    }
}
