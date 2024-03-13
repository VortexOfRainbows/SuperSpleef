using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior : MonoBehaviour 
{
    // Start is called before the first frame update

    public Rigidbody enemy;
    public float thrust;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemy.AddForce(transform.up * thrust);
    }
}
