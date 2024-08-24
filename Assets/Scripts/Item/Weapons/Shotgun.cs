
using UnityEngine;

public class Shotgun : Weapon ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetStats()
    {
        ShootSpeed = 15;
        Firerate = 60;
    }
    public override bool Shoot(Player player, Transform direction)
    {
        int shots = 8;
        float spread = 1.45f;
        for(int i = 0; i <= shots; i++)
        {
            float rotation = -(spread * shots / 2) + i * spread;
            direction.Rotate(0, rotation, 0);
            Projectile.NewProjectile(ShootType(player), direction.position, direction.rotation, ShootSpeed * direction.forward + 2.5f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            direction.Rotate(0, -rotation, 0);
        }
        return true;
    }
}
