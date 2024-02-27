using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete] ///This script is currently unused but is planned to be used later
public class Enemy : Entity  ///Team members that contributed to this script: Sehun Heo, Ian Bunnell
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float approachDistance = 10f;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
    }
    public override void OnFixedUpdate()
    {
        //GameObject playerObject = GameObject.FindWithTag(playerTag); //Don't ever do this. Using find function in update is painfully slow and memory intensive
        Player player = FindClosestPlayer(approachDistance);
        if (player != null)
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
