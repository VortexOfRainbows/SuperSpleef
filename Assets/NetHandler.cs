using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetHandler : MonoBehaviour //Team members that contributed to this script: Ian Bunnel
{
    [SerializeField] private UnityTransport UnityTransport;
    /// <summary>
    /// Returns true if in a multiplayer game scene
    /// </summary>
    public static bool Active => NetworkManager.Singleton != null && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer);
    public static string IP
    {
        get
        {
            return Instance.UnityTransport.ConnectionData.Address;
        }
        private set
        {
            Debug.Log("IP is now: " + value);
            Instance.UnityTransport.ConnectionData.Address = value;
        }
    }
    public static ushort Port ///Should always be 7777 for now
    {
        get
        {
            return Instance.UnityTransport.ConnectionData.Port;
        }
        private set
        {
            Instance.UnityTransport.ConnectionData.Port = value;
        }
    }
    public static void StopTryingToConnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
    public void SetIP(string newIP)
    {
        IP = newIP;
    }
    public static NetHandler Instance = null;
    public static List<NetworkPlayer> LoggedPlayers = new List<NetworkPlayer>();
    public static int TotalClients => LoggedPlayers.Count;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    private static bool InitClient = false;
    private static bool InitHost = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }
    private bool AddedListeners = false;
    private void FixedUpdate()
    {
        if(!AddedListeners)
        {
            if (NetHandler.Active)
            {
                AddedListeners = true;
                NetworkManager.Singleton.OnServerStopped += NetworkStopped;
                NetworkManager.Singleton.OnClientStopped += NetworkStopped;
            }
        }
        if (InitHost)
        {
            NetworkManager.Singleton.StartHost();
            InitHost = false;
            GameStateManager.ResetStates();
        }
        if (InitClient)
        {
            NetworkManager.Singleton.StartClient();
            InitClient = false;
        }
        for (int i = 0; i < LoggedPlayers.Count; i++)
        {
            if (LoggedPlayers[i] == null)
            {
                LoggedPlayers.RemoveAt(i);
            }
        }
        /*if(GameStateManager.Instance == null)
        {
            SceneManager.LoadScene(GameStateManager.TitleScreen);
            Debug.Log("Lost GameStateManager Instance");
        }*/
    }
    private static void HostGame()
    {
        SceneManager.LoadScene(3);
        InitHost = true;
    }
    private static void JoinGame()
    {
        InitClient = true;
    }
    private static void NetworkStopped(bool IDontKnowWhatThisValueIs)
    {
        SceneManager.LoadScene(GameStateManager.TitleScreen);
        Debug.Log("Server Ended, Returning to Main Menu");
    }
}
