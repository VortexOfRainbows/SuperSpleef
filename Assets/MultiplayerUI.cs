using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour
{
    public const string Leave = "Leave Lobby";
    public const string Close = "Close Lobby";
    private const string WaitingOnServer = "Waiting on Host";
    private const string IAmServer = "You are the Host";
    [SerializeField] private GameObject WorldSizeSettings;
    [SerializeField] private GameObject UCISettings;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private Text WaitingForServerDisplay;
    [SerializeField] private Text LoggedDisplay;
    [SerializeField] private Text LeaveLobbyText;
    private void Update()
    {
        if (LoggedDisplay != null)
        {
            LoggedDisplay.text = string.Empty;
            for (int i = 0; i < NetHandler.LoggedPlayers.Count; i++)
            {
                LoggedDisplay.text += (i + 1) + ": " + NetHandler.LoggedPlayers[i].Username + "\n";
            }
        }
        if (WaitingForServerDisplay != null)
            WaitingForServerDisplay.text = NetworkManager.Singleton.IsServer ? IAmServer : WaitingOnServer;
        if (LeaveLobbyText != null)
            LeaveLobbyText.text = NetworkManager.Singleton.IsServer ? Close : Leave;

        UCISettings.SetActive(false);
        StartButton.SetActive(NetworkManager.Singleton.IsServer);
        WorldSizeSettings.SetActive(NetworkManager.Singleton.IsServer);
    }
    public static void LeaveLobby()
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            int index = -1;
            for(int i = 0; i < NetHandler.LoggedPlayers.Count; i++)
            {
                NetworkPlayer nPlayer = NetHandler.LoggedPlayers[i];
                if (nPlayer.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
                GameStateManager.Instance.DespawnNetworkPlayerRpc(index);
        }
        NetworkManager.Singleton.Shutdown();
    }
}
