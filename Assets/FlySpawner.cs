using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete] // This script is no longer used.
public class FlySpawner : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    public GameObject fly;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) //if the E-key is pressed, a killerfly spawns into the game sceme.
            Instantiate(fly);
    }
}
