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
    public static float ParticleMultiplier { get; private set; }
    public static bool LocalMultiplayer { get; private set; }
    public void Awake()
    {
        LocalMultiplayer = false;
        ParticleMultiplier = 1f;
        Mode = GameModeID.None;
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public static void SetParticleMultiplier(float mult)
    {
        ParticleMultiplier = Mathf.Clamp01(mult);
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
        if (mode == GameModeID.LocalMultiplayer || mode == GameModeID.NetMultiplayer)
        {
            LocalMultiplayer = true;
            Mode = mode == GameModeID.NetMultiplayer ? GameModeID.Apocalypse : GameModeID.None;
            SceneManager.LoadScene(2);
        }
        else
        {
            LocalMultiplayer = false;
            SceneManager.LoadScene(1); // Loads the Main Scene (Gameplay Scene)
        }
    }
}
