using Unity.Netcode;
using UnityEngine;

public abstract class Projectile : NetworkBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    private NetworkVariable<Vector3> velocity = new NetworkVariable<Vector3>(Vector3.zero);
    public static GameObject NewProjectile(int ProjectileType, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject pObject = Instantiate(ProjectileManager.GetProjectile(ProjectileType), position, rotation);
            pObject.GetComponent<Rigidbody>().velocity = velocity;
            pObject.GetComponent<NetworkObject>().Spawn();
            return pObject;
        }
        GameStateManager.Instance.SpawnProjectileRpc(ProjectileType, position, rotation, velocity);
        return null;
    }
    protected MeshRenderer mRenderer;
    public Entity owner { get; protected set; }
    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            Vector3 velo = GetComponent<Rigidbody>().velocity;
            Debug.Log("Spawn velo: " + velo);
            velocity.Value = velo;
        }
        mRenderer = GetComponent<MeshRenderer>();
        OnSpawn();
    }
    void Start()
    {
        OnNetworkSpawn();
    }
    /// <summary>
    /// Called when the projectile spawns into the world.
    /// </summary>
    public virtual void OnSpawn()
    {

    }
    private Color prevTint = Color.white;
    private void ModifyColors()
    {
        if (mRenderer == null)
            return;
        Color tint = DrawColor();
        if(tint != prevTint)
        {
            //Debug.Log("recolored!");
            mRenderer.material.SetColor("_BaseColor", tint);
            prevTint = tint;
        }
    }
    /// <summary>
    /// Override this with the color you want the projectile to draw in. Defaults to white.
    /// </summary>
    /// <returns></returns>
    public virtual Color DrawColor()
    {
        return Color.white;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < World.OutOfBounds)
        {
            Kill(true);
            return;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (NetworkManager.Singleton.IsServer)
        {
            Vector3 velo = rb.velocity;
            Debug.Log("update velo: " + velo);
            velocity.Value = velo;
        }
        else
            Debug.Log("client velo: " + velocity.Value);
        rb.velocity = velocity.Value;
        ModifyColors();
        OnFixedUpdate();
    }
    /// <summary>
    /// Called after the normal fixed update actions of the parent projectile.
    /// </summary>
    public virtual void OnFixedUpdate()
    {

    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            CollisionCheck(collision);
    }
    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            CollisionCheck(collision);
    }
    public void CollisionCheck(Collision collision)
    {
        if(OnCollision(collision))
        {
            Kill(false);
            return;
        }
    }
    /// <summary>
    /// Called when the projectile collides with the ground.
    /// Return true to kill the projectile. returns true by default.
    /// </summary>
    /// <param name="collision"></param>
    public virtual bool OnCollision(Collision collision)
    {
        return true;
    }
    /// <summary>
    /// Ran before the object is destroyed. 
    /// OutBoundDeath is whether or not the projectile died due to being out of bounds.
    /// If the OutBoundDeath is true, it died from being out of bounds. False otherwise.  
    /// 
    /// Only called on server
    /// </summary>
    public virtual void OnDeath(bool OutBoundDeath)
    {

    }
    /// <summary>
    /// Kills the projectile
    /// </summary>
    /// <param name="OutBoundDeath"></param>
    public void Kill(bool OutBoundDeath = false)
    {
        if(IsServer)
        {
            OnDeath(OutBoundDeath);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
