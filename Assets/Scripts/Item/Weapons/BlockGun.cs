using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGun : Weapon ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetStats()
    {
        ShootSpeed = 15;
    }
    public override int ShootType(Player player)
    {
        return ProjectileID.BlockProjectile;
    }
}
