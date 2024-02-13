using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameModeID
{
    public const int None = -1;
    public const int Creative = 0;
    public const int Apocalypse = 1;
    public const int LocalMultiplayer = 2;
    public const int NetMultiplayer = 3;
}
public class GameStateManager :MonoBehaviour
{
    public static GameStateManager Instance;
    public static int Mode { get; private set; }
    public void Awake()
    {
        Mode = GameModeID.None;
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public static void MainMenu()
    {
        SceneManager.LoadScene(0); //Loads the SuperSpleef Title Page
    }
    public static void ExitGame()
    {
        Application.Quit(); // Quits the Game
    }
    public static void StartGame(int mode)
    {
        Mode = mode;
        SceneManager.LoadScene(1); // Loads the Main Scene (Gameplay Scene)
    }
}
