using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    [SerializeField] private float acceleration; // The increase in speed per frame
    [SerializeField] private float speed; // how fast the object is going
    [SerializeField] private Transform target; // the target that will be pulled towards the player
    private void OnTriggerStay(Collider col) // When a collider enters the gameobjects trigger collider
    {
        if (col.gameObject.tag == "Item") // If the object is an item...
        {
            speed += acceleration * Time.deltaTime; // Accelerate the target per frame
            target.position = Vector3.MoveTowards(target.position, transform.position, speed); // move the target towards the player
        }
    }
    private void OnTriggerExit(Collider other) // When the item is no longer in range...
    {
        speed = 0; // Stop the object
    }
}
