using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private int PlayerToWatchID = -1;
    private Transform CameraTransform => ClientManager.Camera.transform;
    public string Username => MyName.Value.ToString();
    private NetworkVariable<FixedString512Bytes> MyName = new NetworkVariable<FixedString512Bytes>("Username", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    void Start()
    {
        NetHandler.LoggedPlayers.Add(this);
    }
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            MyName.Value = GameStateManager.LocalUsername;
        }
    }
    public override void OnNetworkDespawn()
    {

    }
    private void Update()
    {
        if(GameStateManager.Players.Count > 0)
        {
            Player myPlayer = null;
            foreach (Player player in GameStateManager.Players)
            {
                if (player.OwnerClientId == OwnerClientId)
                {
                    myPlayer = player;
                    break;
                }
            }
            if(myPlayer != null)
            {
                transform.position = myPlayer.transform.position;
                transform.rotation = myPlayer.transform.rotation;
            }
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    PlayerToWatchID++;
                    if (PlayerToWatchID >= GameStateManager.Players.Count)
                    {
                        PlayerToWatchID = -1;
                    }
                }
                if (PlayerToWatchID < 0)
                {
                    CameraTransform.position = transform.position;
                }
                else
                {
                    Player toWatch = GameStateManager.Players[PlayerToWatchID];
                    transform.position = toWatch.transform.position;
                    transform.rotation = toWatch.transform.rotation;
                }
            }
        }
    }
}
