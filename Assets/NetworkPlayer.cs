using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    void Start()
    {
        NetHandler.LoggedPlayers.Add(this);
    }
    public override void OnNetworkDespawn()
    {

    }
}
