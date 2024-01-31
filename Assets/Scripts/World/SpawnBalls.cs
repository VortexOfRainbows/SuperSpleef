using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 BallCountMinMax = new Vector2(5, 20);
    [SerializeField] private float SpawnTime = 3;
    [SerializeField] private float SpawnSpread = 5f;
    [SerializeField] private float RandomVelocity = 2f;
    [SerializeField] private float SpawnHeight = 70f;
    private float timer;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > SpawnTime) 
        { 
            for (int i = 0; i < Random.Range(BallCountMinMax.x, BallCountMinMax.y); i++)
            {
                GameObject type = ProjectileManager.GetProjectile<BouncyDeathBall>();
                if(Random.value < 0.1)
                    type = ProjectileManager.GetProjectile<FragBall>();
                Rigidbody rb = Instantiate(type, new Vector3(player.transform.position.x + Random.Range(-SpawnSpread, SpawnSpread), SpawnHeight, player.transform.position.z + Random.Range(-SpawnSpread, SpawnSpread)), Quaternion.identity).GetComponent<Rigidbody>();
                rb.velocity = new Vector3(Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity));
            }
            timer = 0;
        }
    }
}
