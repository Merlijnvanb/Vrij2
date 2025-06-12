namespace Quantum
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GameView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public int MadeItSceneIndex;
        public int SacrificedSceneIndex;
        
        void Start()
        {
            QuantumEvent.Subscribe<EventEndGame>(listener: this, handler: HandleEnd);
        }

        // void Update()
        // {
        //     if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        //     {
        //         Debug.Log("Runner get players count: " + QuantumRunner.Default.Game.GetLocalPlayers().Count + ", index 0: " + QuantumRunner.Default.Game.GetLocalPlayers()[0]._index);
        //     }
        // }

        private void HandleEnd(EventEndGame e)
        {
            var playerIndices = QuantumRunner.Default.Game.GetLocalPlayers();
            var localIndex = playerIndices[0]._index;

            if (localIndex == 1)
            {
                if (e.Survivor1MadeIt)
                    SceneManager.LoadScene(MadeItSceneIndex);
                else
                    SceneManager.LoadScene(SacrificedSceneIndex);
            }
            else
            {
                if (e.Survivor2MadeIt)
                    SceneManager.LoadScene(MadeItSceneIndex);
                else
                    SceneManager.LoadScene(SacrificedSceneIndex);
            }

            QuantumRunner.ShutdownAll();
        }
    }
}
