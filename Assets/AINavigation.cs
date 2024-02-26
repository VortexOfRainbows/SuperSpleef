using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
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
