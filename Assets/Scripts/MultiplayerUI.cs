using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour //Team members that contributed to this script: Ian Bunnel
{
    public const string Leave = "Leave Lobby";
    public const string Close = "Close Lobby";
    private const string WaitingOnServer = "Waiting on Host";
    private const string IAmServer = "You are the Host";
    [SerializeField] private GameObject StartButton;
    [SerializeField] private Text WaitingForServerDisplay;
    [SerializeField] private TextMeshProUGUI LoggedDisplay;
    [SerializeField] private Text LeaveLobbyText;
    private void Update()
    {
        if (LoggedDisplay != null)
        {
            LoggedDisplay.text = string.Empty;
            int highestScorer = 1;
            for (int i = 0; i < NetHandler.LoggedPlayers.Count; i++)
            {
                if (NetHandler.LoggedPlayers[i].WinCount.Value > highestScorer)
                {
                    highestScorer = NetHandler.LoggedPlayers[i].WinCount.Value;
                }
            }
            for (int i = 0; i < NetHandler.LoggedPlayers.Count; i++)
            {
                int value = NetHandler.LoggedPlayers[i].WinCount.Value;
                string spriteToUse = value == highestScorer ? " <sprite index=0> " : " <sprite index=1> ";
                string blockGameString = "";
                if (NetHandler.LoggedPlayers[i].BlockGameCount.Value > 0)
                {
                    blockGameString = NetHandler.LoggedPlayers[i].BlockGameCount.Value + " <sprite index=2> ";
                }
                LoggedDisplay.text += value + spriteToUse + NetHandler.LoggedPlayers[i].Username + $"     {blockGameString}" + "\n";
            }
        }
        if (WaitingForServerDisplay != null)
            WaitingForServerDisplay.text = NetworkManager.Singleton.IsServer ? IAmServer : WaitingOnServer;
        if (LeaveLobbyText != null)
            LeaveLobbyText.text = NetworkManager.Singleton.IsServer ? Close : Leave;

        StartButton.SetActive(NetworkManager.Singleton.IsServer);
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
                Main.NetData.DespawnNetworkPlayerRpc(index);
        }
        NetworkManager.Singleton.Shutdown();
    }
}
