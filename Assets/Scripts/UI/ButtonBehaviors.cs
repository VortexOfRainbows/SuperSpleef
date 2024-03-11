using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviors : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    public void MainMenu()
    {
        if (NetworkManager.Singleton != null)
            MultiplayerUI.LeaveLobby();
        else
            GameStateManager.MainMenu(); //Loads the SuperSpleef Title Page
    }
    public void ExitGame()
    {
        GameStateManager.ExitGame(); // Quits the Game
    }
    public void StartGame()
    {
        GameStateManager.StartGame(GameStateManager.Mode); // Loads the Main Scene (Gameplay Scene)
    }
    public void RestartGame()
    {
        GameStateManager.RestartGame();
    }
}
