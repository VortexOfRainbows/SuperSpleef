using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    void Start()
    {
        NetHandler.LoggedPlayers.Add(this);
    }
    void Update()
    {
        
    }
}
