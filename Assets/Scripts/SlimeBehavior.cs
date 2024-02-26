using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeBehavior : Entity
{
    [SerializeField] private int TotalFrags = 8;
    [SerializeField] private float ExplosionSpeedMult = 3f;
    [SerializeField] private float RandomBonusSpread = 1f;

    [SerializeField] private GameObject innerCube;
    [SerializeField] private float innerCubeSizeMult = 0.5f;
    [SerializeField] private float attackInnerCubeSizePercentage = 0.2f;
    private Player target;
    private Rigidbody rb;
    private float jumpTimer;
    private float attackTimer;
    private bool justSlammed;
    [SerializeField] private float approachDistance = 6;
    [SerializeField] private float speed;
    [SerializeField] private float slamRadius = 2;
    [SerializeField] private float slamSpeed = 10;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeBetweenJumps = 1;
    [SerializeField] private float timeBetweenAttacks = 3;
    [SerializeField] private float chaseJumpBonus = 0.5f;
    void Start()
    {
        Inventory = new Inventory(1);
        rb = GetComponent<Rigidbody>();
    }
    public override void OnFixedUpdate()
    {
        attackTimer -= Time.fixedDeltaTime;
        jumpTimer -= Time.fixedDeltaTime;
        innerCube.transform.localScale = innerCubeSizeMult * Vector3.one * (attackInnerCubeSizePercentage + (1 - attackInnerCubeSizePercentage) * Mathf.Clamp(1 - attackTimer / timeBetweenAttacks, 0, 1)); //Change size of cube in the middle depending on how the attack timer is going
        innerCube.transform.localPosition = rb.velocity.normalized * -Mathf.Clamp(rb.velocity.magnitude * innerCubeSizeMult, 0, innerCubeSizeMult / 4.05f); //Makes the inner cube stray slightly behind the movement of the greater cube
        innerCube.transform.rotation = Quaternion.identity;

        ///STATE 3: The slime will slam down on the player and do damage to the environment if a player is near and it is above the player.
        if(Mathf.Abs(rb.velocity.y) < 1) //If roughly at the apex of the jump
        {
            if(attackTimer <= 0 && target != null)
            {
                Vector2 xzToPlayer = new Vector2(target.transform.position.x, target.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
                if(xzToPlayer.magnitude < slamRadius && target.transform.position.y < transform.position.y) //Only do the slame if above the player in some way
                {
                    Attack();
                }
            }
        }
        if(rb.velocity.x != 0 && rb.velocity.z != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, new Vector2(-rb.velocity.x, -rb.velocity.z).ToRotation() * Mathf.Rad2Deg, transform.rotation.z), 0.04f);
    }
    public void Chase(Player player)
    {
        rb.velocity += (player.transform.position + Vector3.up * chaseJumpBonus - transform.position).normalized * speed;
    }
    public void Wander()
    {
        Vector3 wanderTarget = Utils.randVector3Circular(1, 0.5f, 1f).normalized;
        rb.velocity += wanderTarget * speed;
    }
    public void Attack()
    {
        rb.velocity *= 0.1f;
        attackTimer = timeBetweenAttacks;
        rb.velocity += Vector3.down * slamSpeed;
        justSlammed = true;
    }
    public void Multiply() 
    { 
        
    }
    public void Jump()
    {
        rb.velocity *= 0.1f;
        rb.AddForce(Vector3.up * jumpForce);
        jumpTimer = timeBetweenJumps;

        target = null;
        float minDist = approachDistance;
        for (int i = 0; i < GameStateManager.Players.Count; i++)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameStateManager.Players[i].transform.position);
            if (distanceToPlayer < minDist)
            {
                minDist = distanceToPlayer; //This is a simple way of maing the enemy search for the closest player, in cases such as multiplayer
                target = GameStateManager.Players[i];
            }
        }
        ///STATE 1: If a player is near the slime, it will chase the player
        if (target != null) 
        {
            Chase(target);
        }
        ///STATE 2: If a player is not near the slime, it will wander around
        else
        {
            Wander();
        }
        ///STATE 3: The slime will slam down on the player and do damage to the environment if a player is near and it is above the player.
        if (justSlammed)
        {
            justSlammed = false;
            World.FillBlock(transform.position + new Vector3(2, 1, 2), transform.position - new Vector3(2, 1, 2), BlockID.Air, 0.5f);
            for (int i = 0; i < TotalFrags; i++)
            {
                Vector2 circularSpread = new Vector2(ExplosionSpeedMult, 0).RotatedBy(i * 2 * Mathf.PI / TotalFrags);
                Vector3 velo = new Vector3(circularSpread.x, 3 * ExplosionSpeedMult, circularSpread.y); //The balls travel up in a circle pattern around the projectile
                Rigidbody rb = Instantiate(ProjectileManager.GetProjectile(ProjectileID.BouncyDeathBall), transform.position + velo.normalized, Quaternion.identity).GetComponent<Rigidbody>();
                rb.velocity = velo + new Vector3(Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread), Random.Range(-RandomBonusSpread, RandomBonusSpread));
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollision(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (collision.impulse.y > 0) //This essentially checks if the collision is vertical in nature
            {
                if (jumpTimer <= 0)
                    Jump();
            }
        }
    }
}
