using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private bool SecondManager = false;
    [SerializeField] private GameObject RestartButton;
    [SerializeField] private GameObject RestartButton2;
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private Text LeaveLobbyText1;
    [SerializeField] private Text LeaveLobbyText2;
    /// <summary>
    /// A global accessible instance of client manager for general usage
    /// </summary>
    private static ClientManager Instance { get; set; }
    /// <summary>
    /// The second  global accessible instance of client manager for general usage
    /// Only has a value in local multiplayer
    /// </summary>
    private static ClientManager SecondInstance { get; set; }
    public static Camera GetCamera(bool SecondPlayer = false)
    {
        return (SecondPlayer ? SecondInstance : Instance).MainCamera;
    }
    public static ScreenBlocker GetBlocker(bool SecondPlayer = false)
    {
        return (SecondPlayer ? SecondInstance : Instance).ScreenBlocker;
    }
    public static GameObject GetOutline(bool SecondPlayer = false)
    {
        return (SecondPlayer ? SecondInstance : Instance).BlockOutline;
    }
    public static GameObject GetInventoryInterface(bool SecondPlayer = false)
    {
        return (SecondPlayer ? SecondInstance : Instance).InventoryUI;
    }
    [SerializeField] private Camera MainCamera;
    [SerializeField] private ScreenBlocker ScreenBlocker;
    [SerializeField] private GameObject BlockOutline;
    [SerializeField] private string ApocalypseText;
    [SerializeField] private string CreativeText;
    [SerializeField] private string MultiplayerText;
    [SerializeField] private Text BelowTimerText;
    public void Awake()
    {
        if (SecondManager)
            SecondInstance = this;
        else
            Instance = this;
    }
    public void Start()
    {
        //Instance = this;
        if (GameStateManager.LocalMultiplayer)
            BelowTimerText.text = MultiplayerText;
        else if (GameStateManager.Mode == GameModeID.Apocalypse)
            BelowTimerText.text = ApocalypseText;
        else if(GameStateManager.Mode == GameModeID.Creative)
            BelowTimerText.text = CreativeText;
    }
    public void Update()
    {
        if(NetHandler.Active)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                RestartButton.SetActive(true);
                RestartButton2.SetActive(true);
                LeaveLobbyText1.text = LeaveLobbyText2.text = MultiplayerUI.Close;
            }
            else
            {
                RestartButton.SetActive(false);
                RestartButton2.SetActive(false);
                LeaveLobbyText2.text = LeaveLobbyText2.text = MultiplayerUI.Leave;
            }
        }
    }
}
