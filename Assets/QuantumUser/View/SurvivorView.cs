namespace Quantum
{
    using Quantum;
    using UnityEngine;

    public class SurvivorView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Body;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SurvivorData>(EntityRef, out var survivorData))
                return;
            
            var pos = survivorData.Position.ToUnityVector2();
            var facing = survivorData.Facing.ToUnityVector2();
            var facing3D = new Vector3(facing.x, 0, facing.y);
            
            Body.position = new Vector3(pos.x, Body.position.y, pos.y);
            Body.rotation = Quaternion.LookRotation(facing3D, Vector3.up);
        }
    }
}
