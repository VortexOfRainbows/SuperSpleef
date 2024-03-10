using UnityEngine;

public class SpawnBalls : MonoBehaviour ///Team members that contributed to this script: Samuel Gines, Ian Bunnell
{
    [SerializeField] private Entity KillerFly;
    [SerializeField] private Entity Slime;
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

    //Enemies
    [SerializeField] private float SlimeChance = 0.06f;
    [SerializeField] private float FlyChance = 0.06f;
    [SerializeField] private float EnemySpawnPadding = 4;

    private float timer;
    private float TotalTimePassed;
    private void FixedUpdate() ///Summoning falling balls is only for the apocalypse game mode
    {
        if(GameStateManager.Mode != GameModeID.Apocalypse)
        {
            return;
        }
        float scaleMult = TotalTimePassed / SecondsUntilMaxDifficulty;
        if (scaleMult > MaxDifficultyMultiplier)
            scaleMult = MaxDifficultyMultiplier;
        TotalTimePassed += Time.fixedDeltaTime;
        timer += Time.deltaTime * (1 + scaleMult); //Timer goes faster the longer you have been alive
        int pCount = GameStateManager.Players.Count;
        float ballChanceMult = (1 + scaleMult * 2);
        if (timer > SpawnTime && pCount > 0) 
        { 
            foreach(Player player in GameStateManager.Players)
            {
                for (int i = 0; i < Mathf.Lerp(BallCountMinMax.x, BallCountMinMax.y, scaleMult / MaxDifficultyMultiplier) / pCount; i++)
                {
                    int ballType;
                    if (Random.value < FragChance * ballChanceMult)
                    {
                        ballType = ProjectileID.FragBall;
                    }
                    else if (Random.value < SuperFragChance * ballChanceMult)
                    {
                        ballType = ProjectileID.SuperFragBall;
                    }
                    else if (Random.value < TunnelBoreChance * ballChanceMult)
                    {
                        ballType = ProjectileID.TunnelBore;
                    }
                    else if (Random.value < FragFragChance * ballChanceMult)
                    {
                        ballType = ProjectileID.FragFragBall;
                    }
                    else
                    {
                        ballType = ProjectileID.BouncyDeathBall;
                    }
                    float mult = 1;
                    
                    if (i == 0)
                        mult = 0; //Make one ball always guaranteed to spawn right on top of the player.
                    Vector3 spawnPosition = new Vector3(player.transform.position.x + Random.Range(-SpawnSpread, SpawnSpread) * mult, SpawnHeight, player.transform.position.z + Random.Range(-SpawnSpread, SpawnSpread) * mult);
                    Vector3 velo = Vector3.zero;
                    if (i != 0)
                        velo = new Vector3(Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity), Random.Range(-RandomVelocity, RandomVelocity));
                    Projectile.NewProjectile(ballType, spawnPosition, Quaternion.identity, velo);
                }
                timer = 0;
            }
            if(Random.Range(0, 1f) < SlimeChance * (1 + scaleMult * 2))
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(EnemySpawnPadding, Chunk.Width * World.ChunkRadius - EnemySpawnPadding), SpawnHeight, Random.Range(EnemySpawnPadding, Chunk.Width * World.ChunkRadius - EnemySpawnPadding));
                Instantiate(Slime, randomSpawnPosition, Quaternion.identity);
            }
            if(Random.Range(0, 1f) < FlyChance * (1 + scaleMult * 2))
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(EnemySpawnPadding, Chunk.Width * World.ChunkRadius - EnemySpawnPadding), SpawnHeight, Random.Range(EnemySpawnPadding, Chunk.Width * World.ChunkRadius - EnemySpawnPadding));
                Instantiate(KillerFly, randomSpawnPosition, Quaternion.identity);
            }
        }
    }
}
