using UnityEngine;

public class TunnelBore : Projectile
{
    [SerializeField] private float BlockBreakRadius = 3;
    [SerializeField] private float BonusDepth = 10;
    [SerializeField] private Color color;
    [SerializeField] private float SpinSpeed = 3f;
    private Vector3 myFunnySpinModifier;
    public override void OnSpawn()
    {
        myFunnySpinModifier = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }
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
    public override void OnFixedUpdate()
    {
        Vector3 myEulor = transform.eulerAngles;
        myEulor += myFunnySpinModifier * SpinSpeed;
        transform.eulerAngles = myEulor;
    }
}
