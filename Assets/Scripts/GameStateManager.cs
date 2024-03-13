using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

///Team members that contributed to this script: Ian Bunnell, David Bu
public static class GameModeID // Assigns an int as a reference to each game mode.
{
    public const int None = -1;
    public const int Creative = 0;
    public const int Apocalypse = 1;
    public const int LocalMultiplayer = 2;
    public const int LocalMultiplayerApocalypse = 3;
    public const int LaserBattle = 4;
    public const int LaserBattleApocalypse = 5;
}
public class GameStateManager : MonoBehaviour
{
    public static NetData NetData;
    public static GameStateManager Instance;
    public static string LocalUsername { get; set; } = "";
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject netData;
    public const string TitleScreen = "TitleScreen";
    public const string MainScene = "MainScene";
    public const string MultiplayerScene = "MultiplayerScene";
    public const string MultiplayerGameLobby = "MultiplayerGameLobby";
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
    private static int m_mode;
    public static int Mode 
    { 
        get 
        {
            if(NetHandler.Active)
                return NetData.SyncedMode.Value;
            return m_mode;
        } 
        private set
        {
            if (NetHandler.Active)
                NetData.SyncedMode.Value = value;
            m_mode = value;
        } 
    }
    private static bool m_hasSpawnedPlayers;
    public static bool HasSpawnedPlayers
    {
        get
        {
            if (NetHandler.Active)
                return NetData.HasSpawnedPlayers.Value;
            return m_hasSpawnedPlayers;
        }
        private set
        {
            if (NetHandler.Active)
                NetData.HasSpawnedPlayers.Value = value;
            m_hasSpawnedPlayers = value;
        }
    }
    private static float m_worldSizeOverride;
    public static float WorldSizeOverride
    {
        get
        {
            if (NetHandler.Active)
                return NetData.WorldSizeOverride.Value;
            return m_worldSizeOverride;
        }
        private set
        {
            if (NetHandler.Active)
                NetData.WorldSizeOverride.Value = value;
            m_worldSizeOverride = value;
        }
    }
    private static int m_genSeed;
    public static int GenSeed
    {
        get
        {
            if (NetHandler.Active)
                return NetData.GenSeed.Value;
            return m_genSeed;
        }
        private set
        {
            if (NetHandler.Active)
                NetData.GenSeed.Value = value;
            m_genSeed = value;
        }
    }
    private static int m_startingPlayerCount;
    public static int StartingPlayerCount
    {
        get
        {
            if (NetHandler.Active)
                return NetData.StartingPlayerCount.Value;
            return m_startingPlayerCount;
        }
        private set
        {
            if (NetHandler.Active)
                NetData.StartingPlayerCount.Value = value;
            m_startingPlayerCount = value;
        }
    }
    public static float ParticleMultiplier { get; private set; } = 1;
    public static float SensitivityMultiplier { get; private set; } = 1;
    public static float ControllerSensitivityMultiplier { get; private set; } = 1;
    public static float VolumeMultiplier { get; private set; } = 1;
    public static float MusicMultiplier { get; private set; } = 1;
    public static bool settingsDoIGenerateUCI { get; private set; } = true;
    public static bool LocalMultiplayer { get; private set; }
    public void Awake()
    {
        LocalMultiplayer = false;
        ParticleMultiplier = 1f;
        SensitivityMultiplier = 1f;
        ControllerSensitivityMultiplier = 1f;
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else if(this != Instance)
        {
            Destroy(gameObject);
        }
        ResetStates();
    }
    public static void ResetStates()
    {
        Player = new List<Player>();
        GameOverTextColor = Color.white;
        GameOverText = DefaultGameOverText;
        GameEndDelay = GameEndFrameDelay; //This is a arbitrary number set to delay the ending of a game for a bit, so certain functions that need to be run can run.
        GameOver = false;
        Unpause();

        WaitSomeTimeForAssetsToLoad = NoPlayersLeftTimer = 0;
        ResetServerSyncedStates();
    }
    private static void ResetServerSyncedStates()
    {
        if (NetHandler.Active && NetData != null && NetworkManager.Singleton.IsServer)
        {
            NetData.ResetValues();
        }
        else if (!NetHandler.Active) //Reset these values manually if not on the server
        {
            StartingPlayerCount = -1;
            HasSpawnedPlayers = false;
            if(Mode <= GameModeID.None)
                Mode = 0;
            if (GenSeed <= 0)
                GenSeed = Random.Range(0, int.MaxValue);
            if (WorldSizeOverride <= 0)
                WorldSizeOverride = World.DefaultChunkRadius;
        }
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
    public static void SetVolumeMultiplier(float volumeMulti)
    {
        VolumeMultiplier = Mathf.Clamp(volumeMulti, 0f, 1.0f);
    }
    public static void SetMusicMultiplier(float volumeMulti)
    {
        MusicMultiplier = Mathf.Clamp(volumeMulti, 0f, 1.0f);
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
        SceneManager.LoadScene(TitleScreen); //Loads the SuperSpleef Title Page
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
            LocalMultiplayer = true;
            Mode = mode == GameModeID.LocalMultiplayerApocalypse ? GameModeID.Apocalypse : GameModeID.None;
            SceneManager.LoadScene(2);
        }
        else
        {
            LocalMultiplayer = false;
            if(NetHandler.Active)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(MultiplayerScene, LoadSceneMode.Single); 
            }
            else
                SceneManager.LoadScene(1); // Loads the Main Scene (Gameplay Scene)
        }
    }
    public static void RestartGame()
    {
        if (NetHandler.Active)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(MultiplayerGameLobby, LoadSceneMode.Single);
        }
        else
        {
            GenSeed = -1;
            ResetStates();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public static void Pause()
    {
        GamePaused = true; // Sets the boolean statement GameIsPaused to true.
        if (NetHandler.Active)
            return; //Do not pause the game in multiplayer
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
    private static float WaitSomeTimeForAssetsToLoad = 0;
    private static float NoPlayersLeftTimer = 0;
    private static bool HasResetStateSinceReload = true;
    private void LateUpdate()
    {
        //Debug.Log(NetHandler.Active);
        if (SceneManager.GetActiveScene().name == MultiplayerGameLobby || SceneManager.GetActiveScene().name == TitleScreen)
        {
            if (!HasResetStateSinceReload)
            {
                HasResetStateSinceReload = true;
                if (NetworkManager.Singleton.IsServer)
                    GenSeed = -1;
                ResetStates();
            }
        }
        else if (SceneManager.GetActiveScene().name == MultiplayerScene || SceneManager.GetActiveScene().name == MainScene)
        {
            HasResetStateSinceReload = false;
        }
        if (NetworkManager.Singleton.IsServer && NetData == null)
        {
            //Debug.Log("Initiated Net Data");
            GameObject data = Instantiate(netData);
            data.GetComponent<NetworkObject>().Spawn(false);
        }
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i] == null)
            {
                Players.RemoveAt(i);
            }
        }
        /*if(NetData != null)
        {
            Debug.Log("World Size: " + WorldSizeOverride);
            Debug.Log("Gen Rand: " + GenSeed);
        }*/
        if (SceneManager.GetActiveScene().name == MultiplayerScene)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (!HasSpawnedPlayers)
                {
                    StartingPlayerCount = NetHandler.LoggedPlayers.Count;
                    WaitSomeTimeForAssetsToLoad += Time.deltaTime;
                    //Debug.Log(WaitSomeTimeForAssetsToLoad);
                    if (WaitSomeTimeForAssetsToLoad > 0.1)
                    {
                        //Debug.Log("Finished Waiting");
                        //int i = 0;
                        foreach (NetworkPlayer nPlayer in NetHandler.LoggedPlayers)
                        {
                            //Debug.Log(i);
                            GameObject go = Instantiate(Instance.player, new Vector3(World.ChunkRadius * Chunk.Width / 2, Chunk.Height, World.ChunkRadius * Chunk.Width / 2), Quaternion.identity);
                            go.GetComponent<NetworkObject>().SpawnAsPlayerObject(nPlayer.OwnerClientId, true);
                            //i++;
                        }
                        HasSpawnedPlayers = true;
                    }
                }
                //if (WorldSizeOverride.Value <= 0)
                //    WorldSizeOverride.Value = World.DefaultChunkRadius;
            }
            if(HasSpawnedPlayers)
            {
                if (Player.Count == 1)
                {
                    if (StartingPlayerCount == 1)
                    {
                        //Debug.Log("You are alone in a multiplayer lobby..?");
                    }
                    else if (!GameOver)
                    {
                        Player remainingPlayer = Player[0];
                        string VictoryText = "";
                        for (int i = 0; i < NetHandler.LoggedPlayers.Count; i++)
                        {
                            if(remainingPlayer.OwnerClientId == NetHandler.LoggedPlayers[i].OwnerClientId)
                            {
                                if(NetworkManager.Singleton.IsServer && GameEndDelay == GameEndFrameDelay)
                                    NetHandler.LoggedPlayers[i].WinCount.Value++;
                                VictoryText += NetHandler.LoggedPlayers[i].Username;
                                break;
                            }
                        }
                        bool IAmWinner = remainingPlayer.OwnerClientId == NetworkManager.Singleton.LocalClientId;
                        if (IAmWinner)
                            VictoryText = "You Win";
                        else
                            VictoryText += " Wins";
                        EndGame(VictoryText , IAmWinner ? Color.green : Color.red);
                    }
                }
                if (Player.Count <= 0 && !GameOver)
                {
                    NoPlayersLeftTimer += Time.deltaTime;
                    if(NoPlayersLeftTimer > 1)
                    {
                        EndGame(DefaultGameOverText);
                    }
                }
                else
                    NoPlayersLeftTimer = 0;
            }
        }
    }
}
