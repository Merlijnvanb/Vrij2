namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;

    public class LocalInput : MonoBehaviour
    {
        private void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input i = new Quantum.Input();

            i.Left = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.A) : UnityEngine.Input.GetKey(KeyCode.LeftArrow);
            i.Right = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.D) : UnityEngine.Input.GetKey(KeyCode.RightArrow);
            i.Up = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.W) : UnityEngine.Input.GetKey(KeyCode.UpArrow);
            i.Down = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.S) : UnityEngine.Input.GetKey(KeyCode.DownArrow);

            i.Attack = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.LeftControl) : UnityEngine.Input.GetKey(KeyCode.RightShift);
            i.Parry = callback.PlayerSlot == 0 ? UnityEngine.Input.GetKey(KeyCode.Space) : UnityEngine.Input.GetKey(KeyCode.Keypad0);
            
            callback.SetInput(CleanSOCD(i), DeterministicInputFlags.Repeatable);
        }
        
        private Quantum.Input CleanSOCD(Quantum.Input i)
        {
            if (i.Left && i.Right)
            {
                i.Left = false;
                i.Right = false;
            }

            if (i.Up && i.Down)
            {
                i.Up = false;
                i.Down = false;
            }

            return i;
        }
    }
}