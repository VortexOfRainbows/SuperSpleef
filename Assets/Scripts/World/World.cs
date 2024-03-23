using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class World : MonoBehaviour ///Team members that contributed to this script: David Bu, Ian Bunnell
{
    public static int WorldType = 1;
    private static HashSet<Chunk> ReloadRequired = new HashSet<Chunk>();
    public const float OutOfBounds = -40f;
    public static World Instance { get; private set; }
    public GameObject chunkObj;
    private GameObject[,] chunk;
    public const int DefaultChunkRadius = 15;
    public static int ChunkRadius
    {
        get
        {
            return (int)GameStateManager.WorldSizeOverride;
        }
    }
    public const int WorldLayer = 3;
    public static Vector3Int MaxTiles;
    [SerializeField]
    private ParticleSystem BlockParticles;
    public static ParticleSystem BlockParticleRef;
    public static bool WorldGenFinished { get; private set; }
    private void Awake()
    {
        BlockParticleRef = BlockParticles;
        Instance = this;
    }
    private void Start()
    {
        WorldGenFinished = false;
        GenWorld();
    }
    private void GenWorld()
    {
        Random.InitState(GameStateManager.GenSeed);
        MaxTiles = new Vector3Int(ChunkRadius * Chunk.Width, Chunk.Height, ChunkRadius * Chunk.Width);
        chunk = new GameObject[ChunkRadius, ChunkRadius];
        for (int i = 0; i < chunk.GetLength(1); i++)
        {
            for (int j = 0; j < chunk.GetLength(0); j++)
            {
                Vector2Int chunkPos = new Vector2Int(i, j);
                chunk[i, j] = Instantiate(chunkObj, new Vector3(chunkPos.x * Chunk.Width, 0, chunkPos.y * Chunk.Width), Quaternion.identity, transform);
                chunk[i, j].GetComponent<Chunk>().Index = chunkPos;
                chunk[i, j].layer = WorldLayer; //set to world layer
            }
        }
        GenerateFoliage();
        if(GameStateManager.settingsDoIGenerateUCI)
        {
            GenerateUCI();
        }
        for (int i = 0; i < chunk.GetLength(1); i++) //Completes the mesh for the chunk so it is visible and collideable
        {
            for (int j = 0; j < chunk.GetLength(0); j++)
            {
                chunk[i, j].GetComponent<Chunk>().BuildMesh();
            }
        }
        WorldGenFinished = true;
    }
    [SerializeField] private float BushChance = 0.02f;
    [SerializeField] private float TreeChance = 0.0175f;
    [SerializeField] private int WoodIntoLeaves = 2;
    [SerializeField] private float LeavesRadius = 3.0f;
    [SerializeField] private float LeavesVerticalMult = 0.5f;
    [SerializeField] private Vector2Int TreeHeightMinMax = new Vector2Int(3, 5);
    [SerializeField] private Vector2Int LeavesHeightMinMax = new Vector2Int(4, 5);
    [SerializeField] private Vector2Int LeavesWidthMinMax = new Vector2Int(3, 3);
    [SerializeField] private Vector2Int UCIPadding = new Vector2Int(6, 32);
    [SerializeField] private Vector2Int UCIYLevel = new Vector2Int(34, 50);
    private void GenerateFoliage()
    {
        for(int i = 0; i < MaxTiles.x; i++)
        {
            for(int k = 0; k < MaxTiles.z; k++)
            {
                if(Random.Range(0, 1f) < TreeChance)
                {
                    for (int j = MaxTiles.y - 1; j >= 0; j--)
                    {
                        int blockType = Block(i, j - 1, k); //If the block below is not air, and is in fact grass, place a tree
                        if (blockType != BlockID.Air)
                        {
                            if(blockType == BlockID.Grass)
                            {
                                GenerateTree(i, j, k);
                            }
                            if (blockType == BlockID.Sand)
                            {
                                GenerateColumn(i, j, k, 0, Random.Range(2, 6), BlockID.Cactus);
                            }
                            break;
                        }
                    }
                }
                else if (Random.Range(0, 1f) < BushChance)
                {
                    Vector2 fromCenter = new Vector2(MaxTiles.x / 2 - i, MaxTiles.y / 2 - k);
                    float percentFromCenter = fromCenter.magnitude / MaxTiles.x;
                    if(Random.Range(0.2f, 1f) < Mathf.Sqrt(percentFromCenter)) //The farther away the bush spawn spot is from the center, the higher chance it has of spawning
                    {
                        for (int j = MaxTiles.y - 1; j >= 0; j--)
                        {
                            int blockType = Block(i, j - 1, k); //If the block below is not air, and is in fact grass, place a tree
                            if (blockType != BlockID.Air)
                            {
                                if (blockType == BlockID.Grass)
                                {
                                    GenerateBush(i, j - 1, k);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void GenerateBush(int i, int j, int k)
    {
        int leavesHeight = (int)(Random.Range(LeavesHeightMinMax.x, LeavesHeightMinMax.y + 1) / 2f);
        int leavesWidth = (int)(Random.Range(LeavesWidthMinMax.x, LeavesWidthMinMax.y + 1) / 1.5f);
        float size = leavesHeight / 5f + leavesWidth / 3f + LeavesRadius / 5f;
        for (int j2 = 0; j2 < leavesHeight; j2++)
        {
            if (j2 < leavesHeight)
            {
                for (int i2 = -leavesWidth; i2 <= leavesWidth; i2++)
                {
                    for (int k2 = -leavesWidth; k2 <= leavesWidth; k2++)
                    {
                        int blockType = Block(i + i2, j + j2, k + k2);
                        if(blockType == BlockID.Air)
                        {
                            Vector3 offset = new Vector3(i2, k2, j2);
                            if (offset.magnitude < size * Random.Range(0.75f, 1.25f))
                            {
                                SetBlock(i + i2, j + j2, k + k2, BlockID.Leaves);
                            }
                        }
                    }
                }
            }
        }
    }
    private void GenerateTree(int i, int j, int k)
    {
        int height = Random.Range(TreeHeightMinMax.x, TreeHeightMinMax.y + 1);
        int leavesHeight = LeavesHeightMinMax.x;
        int leavesWidth = LeavesWidthMinMax.x;
        for (int j2 = 0; j2 < height + leavesHeight; j2++)
        {
            if (j2 < height)
            {
                SetBlock(i, j + j2, k, BlockID.Wood);
            }
            else if (j2 < height + leavesHeight)
            {
                if (j2 < height + WoodIntoLeaves)
                {
                    SetBlock(i, j + j2, k, BlockID.Wood);
                }
                for (int i2 = -leavesWidth; i2 <= leavesWidth; i2++)
                {
                    for (int k2 = -leavesWidth; k2 <= leavesWidth; k2++)
                    {
                        float distFromBark = Mathf.Abs(i2) + Mathf.Abs(k2) + Mathf.Abs(j2 - height) * LeavesVerticalMult;
                        if (distFromBark < LeavesRadius)
                        {
                            if (i2 != 0 || k2 != 0 || j2 >= height + WoodIntoLeaves)
                                SetBlock(i + i2, j + j2, k + k2, BlockID.Leaves);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Generates the UCI acronym somewhere in the sky of the world.
    /// </summary>
    private void GenerateUCI()
    {
        ///There are a lot of "magic numbers" here because I didn't want to serialize a ton of these values.
        ///They are chosen deliberately, and there is math behind this, but I was just doing this for fun.
        ///Please don't remove points from this. I could easily remove this method completely and it wouldn't impact the gameplay. 
        ///         ;)
  
        //multiplying Z position by 3/4 so it faces the correct direction towards the players
        Vector2Int whereToPutUCILogo = new Vector2Int(Random.Range(UCIPadding.y, MaxTiles.x - UCIPadding.y), Random.Range(UCIPadding.x + MaxTiles.x * 3 / 4, MaxTiles.x - UCIPadding.x));
        int randomY = Random.Range(UCIYLevel.x, UCIYLevel.y); //This should typically put the logo somewhere in the sky!

        ///The math below centers the structure
        whereToPutUCILogo.x += 3;
        int UPosition = whereToPutUCILogo.x - 10; //-14 from position originally
        int CPosition = whereToPutUCILogo.x;
        int IPosition = whereToPutUCILogo.x + 7; //8 away from position

        ///This generates the U shape
        GenerateColumn(UPosition - 3, randomY, whereToPutUCILogo.y, 1, 9, BlockID.BlueBricks);
        GenerateColumn(UPosition, randomY, whereToPutUCILogo.y, 1, 3, BlockID.BlueBricks);
        GenerateColumn(UPosition + 3, randomY, whereToPutUCILogo.y, 1, 9, BlockID.BlueBricks);

        ///This generates the C shape
        GenerateColumn(CPosition - 3, randomY, whereToPutUCILogo.y, 1, 9, BlockID.YellowBricks);
        GenerateColumn(CPosition, randomY, whereToPutUCILogo.y, 1, 3, BlockID.YellowBricks);
        GenerateColumn(CPosition + 3, randomY, whereToPutUCILogo.y, 1, 3, BlockID.YellowBricks);
        GenerateColumn(CPosition, randomY + 6, whereToPutUCILogo.y, 1, 3, BlockID.YellowBricks);
        GenerateColumn(CPosition + 3, randomY + 6, whereToPutUCILogo.y, 1, 3, BlockID.YellowBricks);

        ///This generates the I shape
        GenerateColumn(IPosition, randomY, whereToPutUCILogo.y, 1, 9, BlockID.BlueBricks);
    }
    private void GenerateColumn(int i, int j, int k, int width, int height, int type)
    {
        for (int j2 = 0; j2 < height; j2++)
        {
            for (int i2 = -width; i2 <= width; i2++)
            {
                for (int k2 = -width; k2 <= width; k2++)
                {
                    if (Block(i + i2, j + j2, k + k2) == BlockID.Air)
                    {
                        SetBlock(i + i2, j + j2, k + k2, type);
                    }
                }
            }
        }
    }
    public GameObject GetChunk(int i, int j)
    {
        if (i >= ChunkRadius || j >= ChunkRadius || i < 0 || j < 0)
        {
            return null;
        }
        return chunk[i, j];
    }
    public GameObject BoundingChunk(float x, float z)
    {
        Vector2Int Index = new Vector2Int(Mathf.FloorToInt(x / Chunk.Width), Mathf.FloorToInt(z / Chunk.Width));
        int i = Index.x;
        int j = Index.y;
        return GetChunk(i, j);
    }
    private static void QueueChunkReload(Chunk chunk)
    {
        if(!ReloadRequired.Contains(chunk))
            ReloadRequired.Add(chunk);
    }
    private void Update()
    {
        while (ReloadRequired.Count > 0)
        {
            Chunk first = ReloadRequired.First();
            first.BuildMesh();
            ReloadRequired.Remove(first);
        }
        Instance = this;
    }
    public static int Block(Vector3 pos)
    {
        return Block(pos.x, pos.y, pos.z);
    }
    public static int Block(float x, float y, float z)
    {
        GameObject chunkObj = Instance.BoundingChunk(x, z);
        if (chunkObj != null && y >= 0 && y < Chunk.Height)
        {
            int blockX = Mathf.FloorToInt(x) - Mathf.FloorToInt(chunkObj.transform.position.x);
            int blockY = Mathf.FloorToInt(y);
            int blockZ = Mathf.FloorToInt(z) - Mathf.FloorToInt(chunkObj.transform.position.z);
            Chunk chunk = chunkObj.GetComponent<Chunk>();
            return chunk.blocks[blockX, blockY, blockZ];
        }
        return BlockID.Air;
    }
    public const float DefaultBlockParticleCount = 18f;
    /// <summary>
    /// Generates block particles for all faces a block has.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="blockType"></param>
    public static void GenerateBlockBreakingParticles(Vector3 pos, int blockType, Transform parent = null, float particleMultiplier = 1f)
    {
        BlockMesh mesh = BlockMesh.Get(blockType);
        int totalUniqueFaces = mesh.UniqueFaces.Count;
        float totalParticles = DefaultBlockParticleCount / totalUniqueFaces * particleMultiplier * GameStateManager.ParticleMultiplier;
        //Debug.Log(totalUniqueFaces);
        for(int i = 0; i < totalUniqueFaces; i++)
        {
            bool success = true;
            if (totalParticles < 1) //With a low particle setting, particles should still be able to spawn occassional (unless the rate is ZERO)
                success = Random.Range(0, 1f) < totalParticles;
            if(success)
            {
                float particlesToSpawn = Mathf.Max(1, totalParticles);
                ParticleSystem p = Instantiate(BlockParticleRef, pos, Quaternion.identity, parent);
                p.emission.SetBurst(0, new ParticleSystem.Burst() { count = particlesToSpawn });

                ParticleSystemRenderer r = p.GetComponent<ParticleSystemRenderer>();
                Vector2Int vector = mesh.UniqueFaces[i].UVPos;

                r.material.mainTextureOffset = new Vector2(vector.x / 16f, vector.y / 16f);
                r.material.mainTextureScale = new Vector2(1f / 16f, 1f / 16f);

                p.Play();
                Destroy(p.gameObject, p.main.duration);
            }
        }
    }
    public static void GenerateBlockBreakSound(Vector3 pos, int blockType)
    {
        if (blockType == BlockID.Dirt || blockType == BlockID.Grass)
            AudioManager.PlaySound(SoundID.Dirt, pos, 0.8f, pitchModifier: -0.1f);
        else if (blockType == BlockID.Sand)
            AudioManager.PlaySound(SoundID.Dirt, pos, 0.75f, pitchModifier: 0.2f);
        else if (blockType == BlockID.Wood || blockType == BlockID.Cactus)
            AudioManager.PlaySound(SoundID.Wood, pos);
        else if (blockType == BlockID.Leaves)
            AudioManager.PlaySound(SoundID.Grass, pos, 0.8f);
        else
            AudioManager.PlaySound(SoundID.Stone, pos);
    }
    public static void GenerateBlockPlacingSound(Vector3 pos, int blockType)
    {
        if (blockType == BlockID.Dirt || blockType == BlockID.Grass)
            AudioManager.PlaySound(SoundID.Dirt, pos, pitchModifier: 0.1f);
        else if (blockType == BlockID.Sand)
            AudioManager.PlaySound(SoundID.Dirt, pos, pitchModifier: -0.2f);
        else if (blockType == BlockID.Wood || blockType == BlockID.Cactus)
            AudioManager.PlaySound(SoundID.Wood, pos, pitchModifier: 0.4f);
        else if (blockType == BlockID.Leaves)
            AudioManager.PlaySound(SoundID.Grass, pos, 0.8f, pitchModifier: 0.25f);
        else
            AudioManager.PlaySound(SoundID.Stone, pos, pitchModifier: 0.4f);
    }
    /// <summary>
    /// Sets a block at position xyz to the blockID. Returns true if the block is successfully converted.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="blockID"></param>
    /// <returns></returns>
    public static bool SetBlock(Vector3 pos, int blockID, float particleMultiplier = 1f, bool DoNotSendPacket = false, bool generateSound = false)
    {
        return SetBlock(pos.x, pos.y, pos.z, blockID, particleMultiplier, DoNotSendPacket, generateSound);
    }
    /// <summary>
    /// Sets a block at position xyz to the blockID. Returns true if the block is successfully converted.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="blockType"></param>
    /// <returns></returns>
    public static bool SetBlock(float x, float y, float z, int blockType, float particleMultiplier = 1f, bool DoNotSendPacket = false, bool generateSound = false)
    {
        if (!NetHandler.Active)
            DoNotSendPacket = true;
        GameObject chunkObj = Instance.BoundingChunk(x, z);
        if (chunkObj != null)
        {
            int blockX = Mathf.FloorToInt(x) - Mathf.FloorToInt(chunkObj.transform.position.x);
            int blockY = Mathf.FloorToInt(y);
            int blockZ = Mathf.FloorToInt(z) - Mathf.FloorToInt(chunkObj.transform.position.z);
            Chunk chunk = chunkObj.GetComponent<Chunk>();
            if (blockY < Chunk.Height && blockY >= 0)
            {
                if (chunk.blocks[blockX, blockY, blockZ] == blockType)
                    return false; //Do not place the same block again

                int typeBeforeBreaking = chunk.blocks[blockX, blockY, blockZ];
                chunk.blocks[blockX, blockY, blockZ] = blockType;

                if (!WorldGenFinished)
                {
                    return true;
                }
                Vector3 spawnPos = new Vector3(Mathf.FloorToInt(x) + 0.5f, Mathf.FloorToInt(y) + 0.5f, Mathf.FloorToInt(z) + 0.5f);
                if (blockType == BlockID.Air && particleMultiplier > 0) //If we are breaking the block, generate particles
                {
                    GenerateBlockBreakingParticles(spawnPos, typeBeforeBreaking, Instance.transform, particleMultiplier);
                    if(generateSound)
                    {
                        GenerateBlockBreakSound(spawnPos, typeBeforeBreaking);
                    }
                }
                if(typeBeforeBreaking == BlockID.Air)
                {
                    if (generateSound)
                    {
                        GenerateBlockPlacingSound(spawnPos, blockType);
                    }
                }
                if (blockX <= 0)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x - 1, chunk.Index.y);
                    if (adjacentChunk != null)
                        QueueChunkReload(adjacentChunk.GetComponent<Chunk>());
                }
                if (blockZ <= 0)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x, chunk.Index.y - 1);
                    if (adjacentChunk != null)
                        QueueChunkReload(adjacentChunk.GetComponent<Chunk>());
                }
                if (blockX >= Chunk.Width - 1)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x + 1, chunk.Index.y);
                    if (adjacentChunk != null)
                        QueueChunkReload(adjacentChunk.GetComponent<Chunk>());
                }
                if (blockZ >= Chunk.Width - 1)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1);
                    if (adjacentChunk != null)
                        QueueChunkReload(adjacentChunk.GetComponent<Chunk>());
                }
                QueueChunkReload(chunk);
                if (!DoNotSendPacket)
                {
                    GameStateManager.NetData.SetBlockRpc(x, y, z, blockType, particleMultiplier, generateSound, GameStateManager.NetData.RpcTarget.Not(NetworkManager.Singleton.LocalClientId, RpcTargetUse.Temp));
                }
                return true;
            }
        }
        return false;
    }
    public static bool FillBlock(Vector3 start, Vector3 end, int blockID, float particleMultiplier = 1f, bool DoNotSendPacket = false, bool generateSound = false)
    {
        return FillBlock(start.x, start.y, start.z, end.x, end.y, end.z, blockID, particleMultiplier, DoNotSendPacket, generateSound);
    }
    public static bool FillBlock(float x, float y, float z, float x2, float y2, float z2, int blockID, float particleMultiplier = 1f, bool DoNotSendPacket = false, bool generateSound = false)
    {
        int X = Mathf.FloorToInt(x);
        int Y = Mathf.FloorToInt(y);
        int Z = Mathf.FloorToInt(z);
        int X2 = Mathf.FloorToInt(x2);
        int Y2 = Mathf.FloorToInt(y2);
        int Z2 = Mathf.FloorToInt(z2);
        return FillBlock(X, Y, Z, X2, Y2, Z2, blockID, particleMultiplier, DoNotSendPacket, generateSound);
    }
    /// <summary>
    /// Fills the area from xyz to x2y2z2 with blockID. Returns true if any blocks are converted. Returns false if no blocks are converted.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="z2"></param>
    /// <param name="blockID"></param>
    /// <returns></returns>
    public static bool FillBlock(int x, int y, int z, int x2, int y2, int z2, int blockID, float particleMultiplier = 1f, bool DoNotSendPacket = false, bool generateSound = false)
    {
        if (!NetHandler.Active)
            DoNotSendPacket = true;
        //Vector3 v = new Vector3(x, y, z);
        //Vector3 v2 = new Vector3(x2, y2, z2);
        //Debug.Log(v + ":" + v2);
        int sX = Mathf.Min(x, x2);
        int sY = Mathf.Min(y, y2);
        int sZ = Mathf.Min(z, z2);
        int bX = Mathf.Max(x, x2);
        int bY = Mathf.Max(y, y2);
        int bZ = Mathf.Max(z, z2);
        bool hasFilledOneBlock = false;
        for (int left = sX; left <= bX; left++)
        {
            for (int bot = sY; bot <= bY; bot++)
            {
                for (int back = sZ; back <= bZ; back++)
                {
                    if(SetBlock(left, bot, back, blockID, particleMultiplier, true, generateSound))
                        hasFilledOneBlock = true;
                    //Debug.Log(left + ":" + bot + ":" + back);
                }
            }
        }
        if(!DoNotSendPacket)
        {
            GameStateManager.NetData.TileFillRpc(x, y, z, x2, y2, z2, blockID, particleMultiplier, generateSound, GameStateManager.NetData.RpcTarget.Not(NetworkManager.Singleton.LocalClientId, RpcTargetUse.Temp));
        }
        return hasFilledOneBlock;
    }
}
