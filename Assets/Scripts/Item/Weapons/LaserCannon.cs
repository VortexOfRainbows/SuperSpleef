using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannon : Weapon ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetStats()
    {
        ShootSpeed = 16;
    }
    public override int ShootType(Player player)
    {
        return ProjectileID.LaserCube;
    }
}
