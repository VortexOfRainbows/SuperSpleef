using UnityEngine;

public class FragBall : Projectile
{
    public override bool OnCollision(Collision collision)
    {
        return true;
    }
    public override Color DrawColor()
    {
        return Color.blue;
    }
    public override void OnDeath(bool OutBoundDeath)
    {
        if(!OutBoundDeath)
            World.FillBlock(transform.position - Vector3.one, transform.position + Vector3.one, BlockID.Air);
    }
}
