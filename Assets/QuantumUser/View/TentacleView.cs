namespace Quantum
{
    using UnityEngine;

    public class TentacleView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform TentacleTransform;

        private float baseY;

        void Start()
        {
            QuantumEvent.Subscribe<EventTentacleMove>(listener: this, handler: HandleMove);
            
            baseY = TentacleTransform.position.y;
        }

        private void HandleMove(EventTentacleMove e)
        {
            var newPos = e.NewPos.ToUnityVector2();
            TentacleTransform.position = new Vector3(newPos.x, baseY, newPos.y);
        }
    }
}
