using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is currently unused.
/// </summary>
[Obsolete]
public class FlySpawner : MonoBehaviour
{
    [SerializeField] private GameObject fly;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
            Instantiate(fly);
    }
}
