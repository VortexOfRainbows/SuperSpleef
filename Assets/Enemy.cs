using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity  ///Team members that contributed to this script: Sehun Heo, Ian Bunnell
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float approachDistance = 10f;
    private Rigidbody rb;
    private void Start()
    {
        Inventory = new Inventory(1);
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        //GameObject playerObject = GameObject.FindWithTag(playerTag); //Don't ever do this. Using find function in update is painfully slow and memory intensive
        Player player = null;
        float minDist = approachDistance;
        for(int i = 0; i < GameStateManager.Players.Count; i++)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameStateManager.Players[i].transform.position);
            if(distanceToPlayer < minDist)
            {
                minDist = distanceToPlayer; //This is a simple way of maing the enemy search for the closest player, in cases such as multiplayer
                player = GameStateManager.Players[i];
            }
        }
        if(player != null)
        {
            Approach(player.transform.position);
        }
    }

    private void Approach(Vector3 location)
    {
        Vector3 direction = (location - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
