using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FlyBehavior : Entity
{
    [SerializeField] private int TotalFrags = 8;
    [SerializeField] private float ExplosionSpeedMult = 3f;
    [SerializeField] private float RandomBonusSpread = 1f;

    private Player target;
    private Rigidbody rb;

    [SerializeField] private float approachDistance = 6;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool selfDestruct;

    [SerializeField] private float detonationTimer;

    public Vector3 wanderTarget;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wanderTarget = new Vector3(Random.Range(0, 100), Random.Range(50, 50), Random.Range(0, 100));
}
    private void FixedUpdate()
    {
        /*if (target != null)
        {
            Chase(target);
        }
        ///STATE 2: If a player is not near the slime, it will wander around
        else
        {
            Wander();
        }*/

        //SelfDestruct(target);
        
        /*if (selfDestruct == false && detonationTimer > 0)
        {
            Chase(target);
            detonationTimer -= Time.deltaTime;
        }
        else if(detonationTimer < 0 || selfDestruct == true)
        {
            selfDestruct = true;
            SelfDestruct(target);
            detonationTimer = 1;
        }*/

        //Debug.Log(detonationTimer);

        SelfDestruct(target);

    }

    public void SelfDestruct(Player player) // We want the entity to wind the attack, then move very fast.
    {
        target = GameStateManager.Players[0];
        selfDestruct = true;
        
        rb.velocity = transform.forward * speed;
        speed = Mathf.Clamp(speed + acceleration, -100, 50);
        //acceleration = (acceleration/ (1.01f)) * Time.deltaTime;

        /*if (speed == 0)
        {
            speed = 0f;
            acceleration = 0f;
            attackdelay += Time.deltaTime;
        }
        
        if (attackdelay >= 1) 
        {
            acceleration = 10;
        }*/

        if (speed < 0) 
        {
            transform.LookAt(player.transform.position);
        }
        else if (speed > 0) 
        {
            Quaternion setRotation = transform.rotation;
            transform.rotation = setRotation;
        }
    }

    public void Chase(Player player) 
    {
        target = GameStateManager.Players[0];
        speed = 5f;
        rb.velocity = transform.forward * speed;
        transform.LookAt(player.transform.position);
    }

    public float nextMovement = 3;
    public void Wander()
    {
        
        rb.velocity = transform.forward * speed;
        float angleBetween = Vector3.Angle(rb.transform.position, wanderTarget);

        if (angleBetween < 20)
        {
            speed = 0;
            nextMovement -= 1f * Time.deltaTime;
            transform.LookAt(wanderTarget);
            Debug.Log(true);
        }
        else 
        {
            speed = 10f;
            transform.LookAt(wanderTarget);
        }
        
        if (nextMovement < 0) 
        {
            speed = 10f;
            nextMovement = 3;
            wanderTarget = new Vector3(Random.Range(0, 150), Random.Range(50, 50), Random.Range(0, 150));
        }
    }

    public void Circle() 
    { 
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (selfDestruct) 
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 50);
            
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(3000f, explosionPos, 50, 3.0F);
            }
            
            World.FillBlock(transform.position + new Vector3(3, 4, 3), transform.position - new Vector3(3, 4, 3), BlockID.Air, 0.5f);
            Destroy(rb.gameObject); 
        }
        
    }
}
