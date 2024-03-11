using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] private GameObject StartButton;
    [SerializeField] private Text LoggedDisplay;
    private void Update()
    {
        if (LoggedDisplay != null)
            LoggedDisplay.text = NetHandler.TotalClients.ToString();
        StartButton.SetActive(NetworkManager.Singleton.IsServer);
    }
    public void LeaveLobby()
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
                GameStateManager.Instance.DespawnPlayerRpc(index);
        }
        NetworkManager.Singleton.Shutdown();
    }
}
