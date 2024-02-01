using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    [SerializeField] private float acceleration;
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Item") 
        {
            speed += acceleration * Time.deltaTime;
            target.position = Vector3.MoveTowards(target.position, transform.position, speed);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        speed = 0;
    }
}
