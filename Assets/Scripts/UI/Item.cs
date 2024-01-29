using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    void OnTriggerEnter(Collider col) 
    {
        if (col.gameObject.tag == "Player") 
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
