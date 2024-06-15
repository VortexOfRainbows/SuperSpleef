using System;
using Unity.Netcode;
using UnityEngine;

public class SafeNetworkVariable<T>
{
    public NetworkVariable<T> NetData;
    public T LocalData;
    public SafeNetworkVariable(NetworkVariable<T> defaultValue)
    {
        LocalData = defaultValue.Value;
        NetData = defaultValue;
    }
    public T Value
    {
        get
        {
            if (NetHandler.Active)
                return NetData.Value;
            return LocalData;
        }
        set
        {
            if (NetHandler.Active)
                NetData.Value = value;
            LocalData = value;
        }
    }
    public static implicit operator T(SafeNetworkVariable<T> networkVariable)
    {
        return networkVariable.Value;
    }
    public static implicit operator SafeNetworkVariable<T>(NetworkVariable<T> networkVariable)
    {
        return new SafeNetworkVariable<T>(networkVariable);
    }
}

///Team members that contributed to this script: Ian Bunnell
public abstract class Entity : NetworkBehaviour //A monobehavior class that possesses an inventory
{
    public SafeNetworkVariable<Vector3> Velocity;
    public NetworkVariable<Vector3> n;
    private void Awake()
    {
        Velocity = n = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }
    public bool MovingForward { get; protected set; }
    public bool MovingBackward { get; protected set; }
    public bool MovingLeft { get; protected set; }
    public bool MovingRight { get; protected set; }
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
        if (!NetHandler.Active || IsServer)
        {
            OnDeath(OutBoundDeath);
            if (NetHandler.Active)
            {
                NetworkObject nObject = GetComponent<NetworkObject>();
                if (nObject.IsSpawned)
                    nObject.Despawn();
                else
                    Destroy(gameObject);
            }
            else
                Destroy(gameObject);
        }
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
