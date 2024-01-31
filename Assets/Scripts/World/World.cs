using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class World : MonoBehaviour
{
    private static HashSet<Chunk> ReloadRequired = new HashSet<Chunk>();
    public const float OutOfBounds = -40f;
    public static World Instance { get; private set; }
    public GameObject chunkObj;
    private GameObject[,] chunk;
    public const int ChunkRadius = 15;
    public const int WorldLayer = 3;
    [SerializeField]
    private ParticleSystem BlockParticles;
    public static ParticleSystem BlockParticleRef;
    private void Awake()
    {
        BlockParticleRef = BlockParticles;
    }
    private void Start()
    {
        Instance = this;
        chunk = new GameObject[ChunkRadius, ChunkRadius];
        for (int i = 0; i < chunk.GetLength(1); i++)
        {
            for(int j = 0; j < chunk.GetLength(0); j++)
            {
                Vector2Int chunkPos = new Vector2Int(i, j);
                chunk[i, j] = Instantiate(chunkObj, new Vector3(chunkPos.x * Chunk.Width, 0, chunkPos.y * Chunk.Width), Quaternion.identity, transform);
                chunk[i, j].GetComponent<Chunk>().Index = chunkPos;
                chunk[i, j].layer = WorldLayer; //set to world layer
            }
        }
        for (int i = 0; i < chunk.GetLength(1); i++)
        {
            for (int j = 0; j < chunk.GetLength(0); j++)
            {
                chunk[i, j].GetComponent<Chunk>().BuildMesh();
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
    public static bool SetBlock(Vector3 pos, int blockID, bool ReleaseParticles = true)
    {
        return SetBlock(pos.x, pos.y, pos.z, blockID, ReleaseParticles);
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
    /// <summary>
    /// Sets a block at position xyz to the blockID. Returns true if the block is successfully converted.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="blockID"></param>
    /// <returns></returns>
    public static bool SetBlock(float x, float y, float z, int blockID, bool ReleaseParticles = true)
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
                if (chunk.blocks[blockX, blockY, blockZ] == blockID)
                    return false; //Do not place the same block again

                chunk.blocks[blockX, blockY, blockZ] = blockID;

                if(blockID == BlockID.Air && ReleaseParticles) //If we are breaking the block, generate particles
                {
                    ParticleSystem p = Instantiate(BlockParticleRef, new Vector3(Mathf.FloorToInt(x) + 0.5f, Mathf.FloorToInt(y) + 0.5f, Mathf.FloorToInt(z) + 0.5f), Quaternion.identity, Instance.transform);
                    p.Play();
                    Destroy(p.gameObject, p.main.duration);
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
    public static bool FillBlock(Vector3 start, Vector3 end, int blockID, bool ReleaseParticles = true)
    {
        return FillBlock(start.x, start.y, start.z, end.x, end.y, end.z, blockID, ReleaseParticles);
    }
    public static bool FillBlock(float x, float y, float z, float x2, float y2, float z2, int blockID, bool ReleaseParticles = true)
    {
        int X = Mathf.FloorToInt(x);
        int Y = Mathf.FloorToInt(y);
        int Z = Mathf.FloorToInt(z);
        int X2 = Mathf.FloorToInt(x2);
        int Y2 = Mathf.FloorToInt(y2);
        int Z2 = Mathf.FloorToInt(z2);
        return FillBlock(X, Y, Z, X2, Y2, Z2, blockID, ReleaseParticles);
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
    public static bool FillBlock(int x, int y, int z, int x2, int y2, int z2, int blockID, bool ReleaseParticles = true)
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
                    if(SetBlock(left, bot, back, blockID, ReleaseParticles))
                        hasFilledOneBlock = true;
                    //Debug.Log(left + ":" + bot + ":" + back);
                }
            }
        }
        return hasFilledOneBlock;
    }
}
