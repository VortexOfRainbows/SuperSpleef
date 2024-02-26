using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///Team members that contributed to this script: Ian Bunnell
public abstract class Entity : MonoBehaviour //A monobehavior class that possesses an inventory
{
    //This class will be used for Enemies, Players, Chests, etc.
    public Inventory Inventory { get; protected set; }
    private void FixedUpdate()
    {
        if(this is not Player)
        {
            if (transform.position.y < World.OutOfBounds)
            {
                Kill(true);
                return;
            }
        }
        OnFixedUpdate();
    }
    public virtual void OnDeath(bool OutBoundDeath)
    {

    }
    /// <summary>
    /// Called after the normal fixed update actions of the parent projectile.
    /// </summary>
    public virtual void OnFixedUpdate()
    {

    }
    /// <summary>
    /// Kills the projectile
    /// </summary>
    /// <param name="OutBoundDeath"></param>
    public void Kill(bool OutBoundDeath = false)
    {
        OnDeath(OutBoundDeath);
        Destroy(gameObject);
    }
}
