using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Obsolete] // This script is no longer used.
public class AINavigation : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    // Start is called before the first frame update

    private NavMeshAgent agent;
    public GameObject player;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Seek(Vector3 location) 
    {
        agent.SetDestination(location);
    }
    void Update()
    {
       Seek(player.transform.position);
    }
}
