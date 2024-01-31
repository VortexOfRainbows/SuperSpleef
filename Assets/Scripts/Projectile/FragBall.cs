using UnityEngine;

public class FragBall : Projectile
{
    [SerializeField] private float TotalFrags = 8;
    [SerializeField] private float ExplosionSpeedMult = 3f;
    [SerializeField] private float RandomBonusSpread = 0.5f;
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
        {
            World.FillBlock(transform.position - Vector3.one * 2, transform.position + Vector3.one * 2, BlockID.Air); //Breaks blocks in a 5x5 area
            for (int i = 0; i < TotalFrags; i++) //spawn 8 balls around this projectile
            {
                Vector2 circularSpread = new Vector2(ExplosionSpeedMult, 0).RotatedBy(i * 2 * Mathf.PI / TotalFrags);
                Vector3 velo = new Vector3(circularSpread.x, 2 * ExplosionSpeedMult, circularSpread.y); //The balls travel up in a circle pattern around the projectile
                Rigidbody rb = Instantiate(ProjectileManager.GetProjectile(ProjectileID.BouncyDeathBall), transform.position + velo.normalized, Quaternion.identity).GetComponent<Rigidbody>();
                rb.velocity = velo + new Vector3(Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread));
            }
        }
    }
}
