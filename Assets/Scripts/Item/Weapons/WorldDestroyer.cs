using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDestroyer : Weapon ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetStats()
    {
        ShootSpeed = 25;
    }
    public override int ShootType(Player player)
    {
        return ProjectileID.FragBall;
    }
}
