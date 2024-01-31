using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public const float OutOfBounds = -40f;
    public static World Instance;
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
    private void Update()
    {
        Instance = this;
    }
    public static int Block(Vector3 pos)
    {
        return Block(pos.x, pos.y, pos.z);
    }
    public static bool SetBlock(Vector3 pos, int blockID)
    {
        return SetBlock(pos.x, pos.y, pos.z, blockID);
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
    public static bool SetBlock(float x, float y, float z, int blockID)
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

                if(blockID == BlockID.Air) //If we are breaking the block, generate particles
                {
                    ParticleSystem p = Instantiate(BlockParticleRef, new Vector3(Mathf.FloorToInt(x) + 0.5f, Mathf.FloorToInt(y) + 0.5f, Mathf.FloorToInt(z) + 0.5f), Quaternion.identity, Instance.transform);
                    p.Play();
                    Destroy(p.gameObject, p.main.duration);
                }

                if (blockX <= 0)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x - 1, chunk.Index.y);
                    if (adjacentChunk != null)
                        adjacentChunk.GetComponent<Chunk>().BuildMesh();
                }
                if (blockZ <= 0)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x, chunk.Index.y - 1);
                    if (adjacentChunk != null)
                        adjacentChunk.GetComponent<Chunk>().BuildMesh();
                }
                if (blockX >= Chunk.Width - 1)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x + 1, chunk.Index.y);
                    if (adjacentChunk != null)
                        adjacentChunk.GetComponent<Chunk>().BuildMesh();
                }
                if (blockZ >= Chunk.Width - 1)
                {
                    GameObject adjacentChunk = Instance.GetChunk(chunk.Index.x, chunk.Index.y + 1);
                    if (adjacentChunk != null)
                        adjacentChunk.GetComponent<Chunk>().BuildMesh();
                }
                chunk.BuildMesh();
                return true;
            }
        }
        return false;
    }
}
