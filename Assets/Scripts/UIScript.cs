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
