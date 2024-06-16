using Unity.Netcode;
using UnityEngine;

///Team members that contributed to this script: Ian Bunnell
public abstract class Entity : NetworkBehaviour //A monobehavior class that possesses an inventory
{
    public NetworkVariable<Vector3> Velocity;
    private NetworkVariable<bool> n_MovingForward, n_MovingBackward, n_MovingLeft, n_MovingRight;
    private void Awake()
    {
        Velocity = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        n_MovingForward = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        n_MovingBackward = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        n_MovingLeft = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        n_MovingRight = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }
    public bool MovingForward { 
        get
        {
            return n_MovingForward.Value;
        }
        protected set
        {
            if (IsOwner)
                n_MovingForward.Value = value;
        }
    }
    public bool MovingBackward
    {
        get
        {
            return n_MovingBackward.Value;
        }
        protected set
        {
            if(IsOwner)
                n_MovingBackward.Value = value;
        }
    }
    public bool MovingLeft
    {
        get
        {
            return n_MovingLeft.Value;
        }
        protected set
        {
            if (IsOwner)
                n_MovingLeft.Value = value;
        }
    }
    public bool MovingRight
    {
        get
        {
            return n_MovingRight.Value;
        }
        protected set
        {
            if (IsOwner)
                n_MovingRight.Value = value;
        }
    }
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
