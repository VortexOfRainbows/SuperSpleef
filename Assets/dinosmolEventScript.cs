using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class dinosmolEventScript : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    public void startMoving() 
    {
        m_Rigidbody.velocity = Vector3.forward * -5;

    }

    public void stopMoving()
    {
        m_Rigidbody.velocity = Vector3.forward * 0;

    }


}
