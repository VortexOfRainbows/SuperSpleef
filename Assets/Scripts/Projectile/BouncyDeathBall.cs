using UnityEngine;
using Unity.Netcode;
public class BouncyDeathBall : Projectile ///Team members that contributed to this script: Ian Bunnell
{
    private const int MaxCollisions = 4;
    NetworkVariable<int> TotalCollisions = new NetworkVariable<int>(0);
    public override bool OnCollision(Collision collision)
    {
        if(NetworkManager.Singleton.IsServer)
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
            TotalCollisions.Value++;
        }
        return TotalCollisions.Value > MaxCollisions;
    }
    public override Color DrawColor()
    {
        if(TotalCollisions.Value >= 1)
        { 
            return Color.Lerp(Color.yellow, Color.red, (TotalCollisions.Value - 1) / (float)(MaxCollisions - 1f));
        }
        return Color.white;
    }
}
