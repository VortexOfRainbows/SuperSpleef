using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyDeathBall : Projectile
{
    private const int MaxCollisions = 4;
    private int TotalCollisions = 0;
    public override bool OnCollision(Collision collision)
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
            if (successfullyBrokeABlock)
                break; //Realistically, there will only be one contact with the projectile (since it has a spherical hitbox)
        }
        TotalCollisions++;
        return TotalCollisions > MaxCollisions;
    }
    public override Color DrawColor()
    {
        if(TotalCollisions >= 1)
        { 
            return Color.Lerp(Color.yellow, Color.red, (TotalCollisions - 1) / (float)(MaxCollisions - 1f));
        }
        return Color.white;
    }
}
