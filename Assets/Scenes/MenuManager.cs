using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Load game scene
    public void StartGame()
    {
        SceneManager.LoadScene("Game"); 
        // Replace "Game" with your actual game scene name
    }

    // Exit game
    public void ExitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}
