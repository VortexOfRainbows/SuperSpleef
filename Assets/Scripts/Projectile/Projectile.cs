using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }
    public void OnCollisionStay(Collision collision)
    {
        CollisionCheck(collision);
    }
    public void CollisionCheck(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Vector3 InsideBlock = transform.position - collision.impulse.normalized;
            Vector3 HitPoint = new Vector3(Mathf.FloorToInt(InsideBlock.x) + 0.5f, Mathf.FloorToInt(InsideBlock.y) + 0.5f, Mathf.FloorToInt(InsideBlock.z) + 0.5f);
            World.SetBlock(HitPoint, BlockID.Air);
        }
    }
}
