using UnityEngine;

public class FlyBehavior : Entity ///Team members that contributed to this script: Samuel Gines, Ian Bunnell
{
    [SerializeField] private GameObject inner;
    private bool GroundIsBelow = false;
    private Rigidbody rb;

    [SerializeField] private Vector3 wanderableRange;

    [SerializeField] private float approachDistance;
    [SerializeField] private float speed = 5;
    [SerializeField] private float selfDestructSpeed = -50;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool selfDestruct;
    [SerializeField] private float maxSpeed = 50;

    [SerializeField] private float detonationTimer;
    private Vector3 wanderTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        NewWander();
    }
    private void NewWander() //pick a random nearby position to wander to
    {
        wanderTarget = transform.position + new Vector3(Random.Range(-wanderableRange.x, wanderableRange.x), -Random.Range(0, wanderableRange.y), Random.Range(-wanderableRange.z, wanderableRange.z)); //Slowly wanders down and around the map
        wanderTarget.x = Mathf.Clamp(wanderTarget.x, 0, World.ChunkRadius * Chunk.Width);
        wanderTarget.z = Mathf.Clamp(wanderTarget.z, 0, World.ChunkRadius * Chunk.Width); //Make sure it doesn't wander horizontally outside of the world
    }
    public override void OnFixedUpdate()
    {
        inner.transform.rotation = Quaternion.identity; //The inner should not rotate. It is meant to hold a collider in place that is used for stuff.
        Player player = FindClosestPlayer(approachDistance);
        
        ///STATE 1: If the killer fly is near a player, it will dive into the player like a kamikaze air plane
        if (player != null)
        {
            approachDistance = int.MaxValue; //Now that it has seen the player once, it has always seen the player.
            speed = selfDestructSpeed = Mathf.Clamp(selfDestructSpeed + acceleration * Time.fixedDeltaTime, -maxSpeed, maxSpeed); // the entity reels back, then charges as the player
            rb.velocity = transform.forward * speed;
            selfDestruct = true;
            if (speed < 0)
            {
                transform.LookAt(player.transform.position); // the entity is always facing the player when in winds back.
            }
            else if (speed > 0) // when the entity begins its charge, it should no longer adjust its rotation.
            {
                Quaternion setRotation = transform.rotation; // records the direction of the player before charging forvard.
                transform.rotation = setRotation;
            }
        }
        else
        {
            ///STATE 3: If there is ground 10 blocks below this (based on the size of a collider): Hover above the ground to reach a higher altitude.
            if (GroundIsBelow) 
            {
                GoUp(); //go up as to hover over the blocks
            }
            ///STATE 2: If there is NOT ground 10 blocks below this (based on the size of a collider): Decend while wandering around randomly
            else 
            {
                Wander();
            }
        }
        GroundIsBelow = false;
    }
    [SerializeField] private float TimeUntilMove = 3;
    private float nextMovement;
    public void Wander()
    {
        float distance = Vector3.Distance(transform.position, wanderTarget);
        if (nextMovement <= 0)
        {
            transform.LookAt(wanderTarget);
            if (distance > 2) //if roughly around the destination
            {
                float speedToMove = speed;
                if (speedToMove > distance)
                    speedToMove = distance;
                rb.velocity = transform.forward * speedToMove;
            }
            else
            {
                NewWander();
                nextMovement = TimeUntilMove;
                transform.LookAt(wanderTarget);
            }
        }
        else
        {
            rb.velocity *= 0.8f; //Slow down while anticipating next movement.
        }
        nextMovement -= Time.fixedDeltaTime;
    }
    public void GoUp()
    {
        rb.velocity = Vector3.up * speed;
        if(nextMovement != 0)
        {
            NewWander();
            nextMovement = 0; //Reset the wander timer so it will wander right away again
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
            if (selfDestruct)
            {
                Kill(false);
            }
        }
    }
    public override void OnDeath(bool OutBoundDeath)
    {
        if(!OutBoundDeath)
            World.FillBlock(transform.position + new Vector3(2, 3, 2), transform.position - new Vector3(2, 3, 2), BlockID.Air, 0.1f);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            GroundIsBelow = true;
        }
    }
}
