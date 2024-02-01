using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemDestruction : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    void OnTriggerEnter(Collider col) // Runs following code if any collider "col" enters the trigger collider attatch to this script
    {
        if (col.gameObject.tag == "Player") // Checks if the item is that interacted with the trigger is a Player
        {
            Destroy(transform.parent.gameObject); // Destroys the trigger radius and the parent object its attached to
        }
    }
}
