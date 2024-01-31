using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDestroyer : Weapon
{
    public override void SetStats()
    {
        ShootSpeed = 25;
    }
    public override GameObject ShootType(Player player)
    {
        return ProjectileManager.GetProjectile(ProjectileID.FragBall);
    }
}
