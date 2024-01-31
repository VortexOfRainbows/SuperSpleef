using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < World.OutOfBounds)
        {
            Destroy(gameObject);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            CollisionCheck(collision);
    }
    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            CollisionCheck(collision);
    }
    public void CollisionCheck(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            ContactPoint cPoint = collision.GetContact(i);
            /*if (!cPoint.otherCollider.CompareTag("Ground"))
            {
                Debug.Log("Warning: collider is not a ground tag");
                continue;
            }*/
            Vector3 point = cPoint.point - cPoint.normal * 0.1f;
            Vector3 InsideBlock = point;
            Vector3 HitPoint = new Vector3(Mathf.FloorToInt(InsideBlock.x) + 0.5f, Mathf.FloorToInt(InsideBlock.y) + 0.5f, Mathf.FloorToInt(InsideBlock.z) + 0.5f);
            bool successfullyBrokeABlock = World.SetBlock(HitPoint, BlockID.Air);
            if(successfullyBrokeABlock)
                break; //Realistically, there will only be one contact with the projectile (since it has a spherical hitbox)
        }
    }
}
