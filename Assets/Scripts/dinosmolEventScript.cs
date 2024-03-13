using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Obsolete] // This script is no longer used.
public class dinosmolEventScript : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    // Start is called before the first frame update

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); // Assigns a reference to the rigibody attached to the Gameobject 
    }
    public void startMoving() 
    {
        m_Rigidbody.velocity = Vector3.forward * -5; // Technically moves forwards, since the orientiations is messed up.

    }

    public void stopMoving()
    {
        m_Rigidbody.velocity = Vector3.forward * 0; // Stops moving

    }


}
