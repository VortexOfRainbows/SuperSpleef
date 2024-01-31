using UnityEngine;

public class TunnelBore : Projectile
{
    [SerializeField] private float BlockBreakRadius = 3;
    [SerializeField] private float BonusDepth = 10;
    [SerializeField] private Color color;
    public override bool OnCollision(Collision collision)
    {
        return true;
    }
    public override Color DrawColor()
    {
        return color;
    }
    public override void OnDeath(bool OutBoundDeath)
    {
        if(!OutBoundDeath)
        {
            World.FillBlock(transform.position - Vector3.one * BlockBreakRadius, transform.position + Vector3.one * BlockBreakRadius, BlockID.Air, true); //Breaks blocks in an area
            World.FillBlock(transform.position - Vector3.one * BlockBreakRadius + Vector3.down * BonusDepth, transform.position + Vector3.one * BlockBreakRadius, BlockID.Air, false); //Blocks broken after that don't generate particles (reduce lag)
        }
    }
}
