namespace Quantum
{
    using Quantum;
    using UnityEngine;
    using Unity.Cinemachine;

    public class SurvivorView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Body;
        public CinemachineTargetGroup TargetGroup;

        private bool groupAssigned = false;

        void Start()
        {
            TargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
            groupAssigned = true;
        }

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;
            
            var pos = survivorData.Position.ToUnityVector2();
            var facing = survivorData.Facing.ToUnityVector2();
            var facing3D = new Vector3(facing.x, 0, facing.y);
            
            Body.position = new Vector3(pos.x, Body.position.y, pos.y);
            Body.rotation = Quaternion.LookRotation(facing3D, Vector3.up);
            
            if (!groupAssigned)
                return;
            
            if (survivorData.SurvivorID == 1)
                TargetGroup.Targets[0].Object = Body;
            else
                TargetGroup.Targets[1].Object = Body;
        }
    }
}
