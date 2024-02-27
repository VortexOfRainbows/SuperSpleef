using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
///Team members that contributed to this script: Ian Bunnell
public abstract class Entity : MonoBehaviour //A monobehavior class that possesses an inventory
{
    //This class will be used for Enemies, Players, Chests, etc.
    public Inventory Inventory { get; protected set; } = new Inventory(0);
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
    /// <summary>
    /// Finds the closest player in range. 
    /// Returns null if no players can be found in the range.
    /// </summary>
    /// <param name="range">Spherical radius to search for a player</param>
    /// <returns></returns>
    public Player FindClosestPlayer(float range)
    {
        Player target = null;
        float minDist = range;
        for (int i = 0; i < GameStateManager.Players.Count; i++)
        {
            if (this is Player && GameStateManager.Players[i] == this)
            {
                continue; //If I am a player looking for a player, do not report me as the closest player.
            }
            float distanceToPlayer = Vector3.Distance(transform.position, GameStateManager.Players[i].transform.position);
            if (distanceToPlayer <= minDist)
            {
                minDist = distanceToPlayer; //This is a simple way of maing the enemy search for the closest player, in cases such as multiplayer
                target = GameStateManager.Players[i];
            }
        }
        return target;
    }
}
