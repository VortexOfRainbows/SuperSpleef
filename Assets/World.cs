using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;
    public GameObject chunkObj;
    GameObject[,] chunk;
    public const int ChunkRadius = 15;
    private void Start()
    {
        Instance = this;
        chunk = new GameObject[ChunkRadius, ChunkRadius];
        for (int i = 0; i < chunk.GetLength(1); i++)
        {
            for(int j = 0; j < chunk.GetLength(0); j++)
            {
                Vector2Int chunkPos = new Vector2Int(i, j);
                chunk[i, j] = Instantiate(chunkObj, new Vector3(chunkPos.x * Chunk.Width, 0, chunkPos.y * Chunk.Width), Quaternion.identity);
                chunk[i, j].GetComponent<Chunk>().Index = chunkPos;
                chunk[i, j].layer = 3; //set to world layer
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
}
