using Unity.Netcode;
using UnityEngine;

public class ButtonBehaviors : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    public void MainMenu()
    {
        if (NetHandler.Active)
            MultiplayerUI.LeaveLobby();
        else
            Main.MainMenu(); //Loads the SuperSpleef Title Page
    }
    public void ExitGame()
    {
        Main.ExitGame(); // Quits the Game
    }
    public void StartGame()
    {
        Main.StartGame(Main.Mode); // Loads the Main Scene (Gameplay Scene)
    }
    public void RestartGame()
    {
        Main.RestartGame();
    }
}
