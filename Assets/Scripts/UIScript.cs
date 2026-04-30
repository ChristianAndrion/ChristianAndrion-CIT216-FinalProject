//4/30/2026
//Christian Andrion
//Handle UI menu buttons

using UnityEngine;

public class UIScript : MonoBehaviour
{

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void TitleScene()
    {
        GameManager.instance.TitleScene();  
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
