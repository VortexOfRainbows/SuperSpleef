using Unity.Netcode;
using UnityEngine;

public class ButtonBehaviors : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    public void MainMenu()
    {
        if (NetHandler.Active)
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
