using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviors : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{

    //Contains all the functions for Every Button that Appears in the UI
    public void MainMenu()
    {
        SceneManager.LoadScene(0); //Loads the SuperSpleef Title Page
    }

    public void ExitGame()
    {
        Application.Quit(); // Quits the Game
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1); // Loads the Main Scene (Gameplay Scene)
    }

}
