using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

///Team members that contributed to this script: Ian Bunnell
public static class GameModeID // Assigns an int as a reference to each game mode.
{
    public const int None = -1;
    public const int Creative = 0;
    public const int Apocalypse = 1;
    public const int LocalMultiplayer = 2;
    public const int LocalMultiplayerApocalypse = 3;
}
public class GameStateManager : NetworkBehaviour
{
    public static NetworkVariable<int> GenSeed;
    [SerializeField] private GameObject player;
    public const string TitleScreen = "TitleScreen";
    public const string MainScene = "MainScene";
    public const string MultiplayerScene = "MultiplayerScene";
    public static List<Player> Players
    {
        get
        {
            if(Player == null)
            {
                Player = new List<Player>();
            }
            return Player;
        }
    }
    private static List<Player> Player;
    public const string DefaultGameOverText = "You Lose";
    public static string GameOverText;
    public static Color GameOverTextColor;
    private const int GameEndFrameDelay = 3;
    private static float GameEndDelay;
    private static bool GameOver;
    private static bool GamePaused;
    public static bool GameIsOver
    {
        get
        {
            return GameOver;
        }
    }
    public static bool GameIsPaused
    {
        get
        {
            return GamePaused;
        }
    }
    public static bool GameIsPausedOrOver
    { 
        get 
        { 
            return GameOver || GamePaused; 
        }
    }
    public static GameStateManager Instance;
    public static int Mode { get; private set; }
    public static float ParticleMultiplier { get; private set; } = 1;
    public static float SensitivityMultiplier { get; private set; } = 1;
    public static float ControllerSensitivityMultiplier { get; private set; } = 1;
    public static float WorldSizeOverride { get; private set; } = World.DefaultChunkRadius;
    public static bool settingsDoIGenerateUCI { get; private set; } = true;
    public static bool LocalMultiplayer { get; private set; }
    public void Awake()
    {
        GenSeed = new NetworkVariable<int>(Random.Range(0, int.MaxValue), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        LocalMultiplayer = false;
        ParticleMultiplier = 1f;
        SensitivityMultiplier = 1f;
        ControllerSensitivityMultiplier = 1f;
        WorldSizeOverride = World.DefaultChunkRadius;
        Mode = GameModeID.None;
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        ResetStates();
    }
    private static void ResetStates()
    {
        Player = new List<Player>();
        GameOverTextColor = Color.white;
        GameOverText = DefaultGameOverText;
        GameEndDelay = GameEndFrameDelay; //This is a arbitrary number set to delay the ending of a game for a bit, so certain functions that need to be run can run.
        GameOver = false;
        Unpause();
    }
    public static void SetParticleMultiplier(float mult)
    {
        ParticleMultiplier = Mathf.Clamp01(mult);
    }
    public static void SetSensitivityMultiplier(float sensMulti)
    {
        SensitivityMultiplier = Mathf.Clamp(sensMulti, 0.01f, 10); //Lowest sensitivity we will let you have is 0.01f. Highest is 10
    }
    public static void SetControllerSensitivityMultiplier(float controlMulti)
    {
        ControllerSensitivityMultiplier = Mathf.Clamp(controlMulti, 0.01f, 10); //Lowest sensitivity we will let you have is 0.01f. Highest is 10
    }
    public static void SetWorldSizeOverride(float size)
    {
        WorldSizeOverride = Mathf.Clamp(size, 1, 100); //Lowest size is 1 chunk. Biggest is 100x100 chunks
    }
    public static void GenerateUCI(bool doIGenerate)
    {
        settingsDoIGenerateUCI = doIGenerate;
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
        ResetStates();
        Mode = mode;
        if (mode == GameModeID.LocalMultiplayer || mode == GameModeID.LocalMultiplayerApocalypse)
        {
            LocalMultiplayer = true; //For now, net and local multiplayer are considered the same (since there is no NET Multiplayer yet)
            Mode = mode == GameModeID.LocalMultiplayerApocalypse ? GameModeID.Apocalypse : GameModeID.None;
            SceneManager.LoadScene(2);
        }
        else
        {
            LocalMultiplayer = false;
            if(NetHandler.OnNetwork)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(MultiplayerScene, LoadSceneMode.Single); 
            }
            else
                SceneManager.LoadScene(1); // Loads the Main Scene (Gameplay Scene)
        }
    }
    public static void RestartGame()
    {
        ResetStates();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void Pause()
    {
        GamePaused = true; // Sets the boolean statement GameIsPaused to true.
        Time.timeScale = 0f; // Freezes the state of the game
    }   
    public static void Unpause()
    {
        GamePaused = false; // Sets the boolean statement GameIsPaused to false.
        Time.timeScale = 1f; // Unfreezes the state of the game
    }
    public static void EndGame(string Subscript, Color color = default)
    {
        if (GameEndDelay > 0)
        {
            GameEndDelay--;
            return;
        }
        GameOverText = Subscript;
        if (color != default)
            GameOverTextColor = color;
        GameOver = true;
        Time.timeScale = 0.5f;
    }
    private float WaitSomeTimeForAssetsToLoad = 0;
    private bool HasSpawnedPlayers = false;
    private void LateUpdate()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (!HasSpawnedPlayers && SceneManager.GetActiveScene().name == MultiplayerScene)
            {
                WaitSomeTimeForAssetsToLoad += Time.deltaTime;
                Debug.Log(WaitSomeTimeForAssetsToLoad);
                if (WaitSomeTimeForAssetsToLoad > 1)
                {
                    Debug.Log("Finished Waiting");
                    int i = 0;
                    foreach (NetworkPlayer nPlayer in NetHandler.LoggedPlayers)
                    {
                        Debug.Log(i);
                        GameObject go = Instantiate(Instance.player, new Vector3(World.ChunkRadius * Chunk.Width / 2, Chunk.Height, World.ChunkRadius * Chunk.Width / 2), Quaternion.identity);
                        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(nPlayer.OwnerClientId);
                        i++;
                    }
                    HasSpawnedPlayers = true;
                }
            }
        }
    }
    
    /// 
    /// Below this point are netcode Rpc's.
    /// These must belong in an instanced class, which is why they are in GameStateManager
    /// Realistically, they should be moved into a new class, but will remain here for now.
    /// 

    [Rpc(SendTo.SpecifiedInParams)]
    public void SetBlockRpc(float x, float y, float z, int type, float particleMultiplier, RpcParams rpcParams)
    {
        World.SetBlock(x, y, z, type, particleMultiplier, true);
    }
    [Rpc(SendTo.SpecifiedInParams)]
    public void TileFillRpc(int x, int y, int z, int x2, int y2, int z2, int blockID, float particleMultiplier, RpcParams rpcParams)
    {
        World.FillBlock(x, y, z, x2, y2, z2, blockID, particleMultiplier, true);
    }
    [Rpc(SendTo.Server)]
    public void SpawnProjectileRpc(int Type, Vector3 pos, Quaternion rot, Vector3 velo)
    {
        Projectile.NewProjectile(Type, pos, rot, velo);
    }
    [Rpc(SendTo.Server)]
    public void DespawnPlayerRpc(int NetworkPlayerIndex)
    {
        NetHandler.LoggedPlayers[NetworkPlayerIndex].GetComponent<NetworkObject>().Despawn();
    }
}
