using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemDestruction : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    void OnTriggerEnter(Collider col) 
    {
        if (col.gameObject.tag == "Player") 
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
