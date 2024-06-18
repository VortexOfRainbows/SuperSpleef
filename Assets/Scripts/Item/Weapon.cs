using UnityEngine;

public abstract class Weapon : Item ///Team members that contributed to this script: Ian Bunnell
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
        AudioManager.PlaySound(SoundID.Weapon, player.transform.position);
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
        Projectile.NewProjectile(ShootType(player), shotTransform.position, shotTransform.rotation, ShootSpeed * shotTransform.forward);
        return true;
    }
    /// <summary>
    /// return the game object that should be generated by the shoot action
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual int ShootType(Player player)
    {
        return ProjectileID.BouncyDeathBall;
    }
}
