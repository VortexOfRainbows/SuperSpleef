using UnityEngine;

public class FragFragBall : Projectile
{
    [SerializeField] private float TotalFrags = 8;
    [SerializeField] private float ExplosionSpeedMult = 3f;
    [SerializeField] private float RandomBonusSpread = 1f;
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
            World.FillBlock(transform.position, transform.position + Vector3.one * 3, BlockID.Air); //Only the top parts of the fill will generate particles
            World.FillBlock(transform.position - Vector3.one * 3, transform.position + Vector3.one * 3, BlockID.Air, false);
            for (int i = 0; i < TotalFrags; i++)
            {
                Vector2 circularSpread = new Vector2(ExplosionSpeedMult, 0).RotatedBy(i * 2 * Mathf.PI / TotalFrags);
                Vector3 velo = new Vector3(circularSpread.x, 2 * ExplosionSpeedMult, circularSpread.y); //The balls travel up in a circle pattern around the projectile
                Rigidbody rb = Instantiate(ProjectileManager.GetProjectile(ProjectileID.FragBall), transform.position + velo.normalized, Quaternion.identity).GetComponent<Rigidbody>();
                rb.velocity = velo + new Vector3(Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread));
            }
        }
    }
}
