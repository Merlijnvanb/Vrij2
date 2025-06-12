using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackToMenu : MonoBehaviour
{
    public int StartMenuIndex;

    public void OnClickGoBack()
    {
        SceneManager.LoadScene(StartMenuIndex);
    }
}
