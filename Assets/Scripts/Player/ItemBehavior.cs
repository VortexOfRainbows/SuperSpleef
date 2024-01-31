using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public float acceleration;
    public float speed;
    public Transform target;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

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
