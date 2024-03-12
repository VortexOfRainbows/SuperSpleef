using UnityEngine;
using Unity.Netcode;
public class BouncyDeathBall : Projectile ///Team members that contributed to this script: Ian Bunnell
{
    private const int MaxCollisions = 4;
    private int m_totalCollisions;
    private NetworkVariable<int> TotalCollisions = new NetworkVariable<int>(0);
    private int CurrentCollisions
    {
        get
        {
            if (NetHandler.Active)
                return TotalCollisions.Value;
            return m_totalCollisions;
        }
        set
        {
            if(NetHandler.Active)
                TotalCollisions.Value = value;
            m_totalCollisions = value;
        }
    }
    public override bool OnCollision(Collision collision)
    {
        AudioManager.PlaySound(SoundID.Wood, transform.position, 0.5f - CurrentCollisions / 10f, pitchModifier: -CurrentCollisions / 10f);
        if (NetworkManager.Singleton.IsServer || !NetHandler.Active)
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
            CurrentCollisions++;
        }
        return CurrentCollisions > MaxCollisions;
    }
    public override Color DrawColor()
    {
        if(CurrentCollisions >= 1)
        { 
            return Color.Lerp(Color.yellow, Color.red, (CurrentCollisions - 1) / (float)(MaxCollisions - 1f));
        }
        return Color.white;
    }
    public override void PlayDeathSound()
    {
        AudioManager.PlaySound(SoundID.Wood, transform.position, 0.5f, pitchModifier: 0.5f);
    }
}
