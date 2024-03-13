using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform //Team members that contributed to this script: Ian Bunnel
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
