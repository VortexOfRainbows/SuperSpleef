using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public float ShootSpeed { get; protected set; }
    public sealed override void SetDefaults()
    {
        Count = 1;
        MaxCount = 1;
        ShootSpeed = 10;
        SetStats();
    }
    public virtual void SetStats()
    {

    }
    public sealed override bool OnPrimaryUse(Player player)
    {
        //Code for melee combat should go here
        return true;
    }
    public sealed override bool OnSecondaryUse(Player player)
    {
        return Shoot(player, player.FacingVector.transform);
    }
    /// <summary>
    /// Called when right click is used with the item
    /// return false if the projectile is not shot, and thus the item is not actually used.
    /// Defaults to true
    /// </summary>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool Shoot(Player player, Transform direction)
    {
        Transform shotTransform = player.FacingVector.transform;
        Rigidbody shotInstance = Instantiate(player.BasicProjectileTest, shotTransform.position, shotTransform.rotation).GetComponent<Rigidbody>();
        shotInstance.velocity = ShootSpeed * shotTransform.forward;// + player.RB.velocity;
        return true;
    }
}
