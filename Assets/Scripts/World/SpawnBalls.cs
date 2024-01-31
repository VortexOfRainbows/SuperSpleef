using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 BallCountMinMax = new Vector2(4, 20);
    [SerializeField] private float SpawnTime = 3;
    [SerializeField] private float SpawnSpread = 5f;
    [SerializeField] private float RandomVelocity = 2f;
    [SerializeField] private float SpawnHeight = 70f;
    [SerializeField] private float SecondsUntilMaxDifficulty = 900;
    [SerializeField] private float FragChance = 0.125f;
    [SerializeField] private float SuperFragChance = 0.025f;
    [SerializeField] private float TunnelBoreChance = 0.025f;
    [SerializeField] private float FragFragChance = 0.025f;
    [SerializeField] private float MaxDifficultyMultiplier = 2f;
    private float timer;
    private float TotalTimePassed;
    private void FixedUpdate()
    {
        float scaleMult = TotalTimePassed / SecondsUntilMaxDifficulty;
        if (scaleMult > MaxDifficultyMultiplier)
            scaleMult = MaxDifficultyMultiplier;
        TotalTimePassed += Time.fixedDeltaTime;
        timer += Time.deltaTime * (1 + scaleMult); //Timer goes faster the longer you have been alive

        float ballChanceMult = (1 + scaleMult * 2);
        if (timer > SpawnTime) 
        { 
            for (int i = 0; i < Mathf.Lerp(BallCountMinMax.x, BallCountMinMax.y, scaleMult / MaxDifficultyMultiplier); i++)
            {
                GameObject ballType;
                if(Random.value < FragChance * ballChanceMult)
                {
                    ballType = ProjectileManager.GetProjectile(ProjectileID.FragBall);
                }
                else if (Random.value < SuperFragChance * ballChanceMult)
                {
                    ballType = ProjectileManager.GetProjectile(ProjectileID.SuperFragBall);
                }
                else if (Random.value < TunnelBoreChance * ballChanceMult)
                {
                    ballType = ProjectileManager.GetProjectile(ProjectileID.TunnelBore);
                }
                else if (Random.value < FragFragChance * ballChanceMult)
                {
                    ballType = ProjectileManager.GetProjectile(ProjectileID.FragFragBall);
                }
                else
                {
                    ballType = ProjectileManager.GetProjectile<BouncyDeathBall>();
                }
                float mult = 1;
                if (i == 0)
                    mult = 0; //Make one ball always guaranteed to spawn right on top of the player.
                Vector3 spawnPosition = new Vector3(player.transform.position.x + Random.Range(-SpawnSpread, SpawnSpread) * mult, SpawnHeight, player.transform.position.z + Random.Range(-SpawnSpread, SpawnSpread) * mult);
                Rigidbody rb = Instantiate(ballType, spawnPosition, Quaternion.identity).GetComponent<Rigidbody>();
                if(i != 0)
                    rb.velocity = new Vector3(Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity));
            }
            timer = 0;
        }
    }
}
