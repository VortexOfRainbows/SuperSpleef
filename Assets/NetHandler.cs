using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetHandler : MonoBehaviour
{
    public static List<NetworkPlayer> LoggedPlayers = new List<NetworkPlayer>();
    public static int TotalClients => LoggedPlayers.Count;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    private static bool InitClient = false;
    private static bool InitHost = false;
    public static bool OnNetwork { get; private set; } = false;
    private void Awake()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }
    private void FixedUpdate()
    {
        if (InitHost)
        {
            NetworkManager.Singleton.StartHost();
            OnNetwork = true;
            InitHost = false;
        }
        if (InitClient)
        {
            NetworkManager.Singleton.StartClient();
            OnNetwork = true;
            InitClient = false;
        }
        for(int i = 0; i < LoggedPlayers.Count; i++)
        {
            if (LoggedPlayers[i] == null)
            {
                LoggedPlayers.RemoveAt(i);
            }
        }
    }
    private static void HostGame()
    {
        SceneManager.LoadScene(3);
        InitHost = true;
    }
    private static void JoinGame()
    {
        SceneManager.LoadScene(3);
        InitClient = true;
    }
}
