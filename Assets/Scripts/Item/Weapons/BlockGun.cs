public class BlockGun : Weapon ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetStats()
    {
        ShootSpeed = 15;
        Firerate = 50;
    }
    public override int ShootType(Player player)
    {
        return ProjectileID.BlockProjectile;
    }
}
