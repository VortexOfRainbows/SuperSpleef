using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGun : Weapon
{
    public override void SetStats()
    {
        ShootSpeed = 15;
    }
    public override GameObject ShootType(Player player)
    {
        return player.BlockProjectileTest;
    }
}
