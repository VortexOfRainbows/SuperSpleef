using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour ///Team members that contributed to this script: David Bu, Ian Bunnell
{
    private static HashSet<Chunk> ReloadRequired = new HashSet<Chunk>();
    public const float OutOfBounds = -40f;
    public static World Instance { get; private set; }
    public GameObject chunkObj;
    private GameObject[,] chunk;
    public const int ChunkRadius = 15; //this is the total number of chunks in each direction xz
    public const int WorldLayer = 3;
    public static Vector3Int MaxTiles;
    [SerializeField]
    private ParticleSystem BlockParticles;
    public static ParticleSystem BlockParticleRef;
    public static bool WorldGenFinished { get; private set; }
    private void Awake()
    {
        BlockParticleRef = BlockParticles;
    }
    private void Start()
    {
        WorldGenFinished = false;
        Instance = this;
        GenWorld();
    }
    private void GenWorld()
    {
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
        GenerateTrees();
        for (int i = 0; i < chunk.GetLength(1); i++) //Completes the mesh for the chunk so it is visible and collideable
        {
            for (int j = 0; j < chunk.GetLength(0); j++)
            {
                chunk[i, j].GetComponent<Chunk>().BuildMesh();
            }
        }
        WorldGenFinished = true;
    }
    [SerializeField] private float TreeChance = 0.0175f;
    [SerializeField] private int WoodIntoLeaves = 2;
    [SerializeField] private float LeavesRadius = 3.0f;
    [SerializeField] private float LeavesVerticalMult = 0.5f;
    [SerializeField] private Vector2Int TreeHeightMinMax = new Vector2Int(3, 5);
    [SerializeField] private Vector2Int LeavesHeightMinMax = new Vector2Int(4, 5);
    [SerializeField] private Vector2Int LeavesWidthMinMax = new Vector2Int(3, 3);
    private void GenerateTrees()
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
                            break;
                        }
                    }
                }
            }
        }
    }
    private void GenerateTree(int i, int j, int k)
    {
        int height = Random.Range(TreeHeightMinMax.x, TreeHeightMinMax.y + 1);
        int leavesHeight = Random.Range(LeavesHeightMinMax.x, LeavesHeightMinMax.y + 1);
        int leavesWidth = Random.Range(LeavesWidthMinMax.x, LeavesWidthMinMax.y + 1);
        for (int j2 = 0; j2 < height + leavesHeight; j2++)
        {
            if (j2 < height)
            {
                SetBlock(i, j + j2, k, BlockID.Wood);
            }
            else if (j2 < height + leavesHeight)
            {
                if(j2 < height + WoodIntoLeaves)
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
    /// <summary>
    /// Sets a block at position xyz to the blockID. Returns true if the block is successfully converted.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="blockID"></param>
    /// <returns></returns>
    public static bool SetBlock(Vector3 pos, int blockID, float particleMultiplier = 1f)
    {
        return SetBlock(pos.x, pos.y, pos.z, blockID, particleMultiplier);
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
    public static void GenerateBlockBreakingParticles(Vector3 pos, int blockType, float particleMultiplier = 1f)
    {
        BlockMesh mesh = BlockMesh.Get(blockType);
        int totalUniqueFaces = mesh.UniqueFaces.Count;
        float totalParticles = DefaultBlockParticleCount / totalUniqueFaces * particleMultiplier;
        //Debug.Log(totalUniqueFaces);
        for(int i = 0; i < totalUniqueFaces; i++)
        {
            ParticleSystem p = Instantiate(BlockParticleRef, new Vector3(Mathf.FloorToInt(pos.x) + 0.5f, Mathf.FloorToInt(pos.y) + 0.5f, Mathf.FloorToInt(pos.z) + 0.5f), Quaternion.identity, Instance.transform);
            p.emission.SetBurst(0, new ParticleSystem.Burst() { count = totalParticles });

            ParticleSystemRenderer r = p.GetComponent<ParticleSystemRenderer>();
            Vector2Int vector = mesh.UniqueFaces[i].UVPos;

            r.material.mainTextureOffset = new Vector2(vector.x / 16f, vector.y / 16f);
            r.material.mainTextureScale = new Vector2(1f / 16f, 1f / 16f);

            p.Play();
            Destroy(p.gameObject, p.main.duration);
        }
    }
    /// <summary>
    /// Sets a block at position xyz to the blockID. Returns true if the block is successfully converted.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="blockType"></param>
    /// <returns></returns>
    public static bool SetBlock(float x, float y, float z, int blockType, float particleMultiplier = 1f)
    {
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
                if(blockType == BlockID.Air && particleMultiplier > 0) //If we are breaking the block, generate particles
                {
                    GenerateBlockBreakingParticles(new Vector3(x, y, z), typeBeforeBreaking, particleMultiplier);
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
                return true;
            }
        }
        return false;
    }
    public static bool FillBlock(Vector3 start, Vector3 end, int blockID, float particleMultiplier = 1f)
    {
        return FillBlock(start.x, start.y, start.z, end.x, end.y, end.z, blockID, particleMultiplier);
    }
    public static bool FillBlock(float x, float y, float z, float x2, float y2, float z2, int blockID, float particleMultiplier = 1f)
    {
        int X = Mathf.FloorToInt(x);
        int Y = Mathf.FloorToInt(y);
        int Z = Mathf.FloorToInt(z);
        int X2 = Mathf.FloorToInt(x2);
        int Y2 = Mathf.FloorToInt(y2);
        int Z2 = Mathf.FloorToInt(z2);
        return FillBlock(X, Y, Z, X2, Y2, Z2, blockID, particleMultiplier);
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
    public static bool FillBlock(int x, int y, int z, int x2, int y2, int z2, int blockID, float particleMultiplier = 1f)
    {
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
                    if(SetBlock(left, bot, back, blockID, particleMultiplier))
                        hasFilledOneBlock = true;
                    //Debug.Log(left + ":" + bot + ":" + back);
                }
            }
        }
        return hasFilledOneBlock;
    }
}
