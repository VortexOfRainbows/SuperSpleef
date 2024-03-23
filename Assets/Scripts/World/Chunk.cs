using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Chunk : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    public const float StoneLayer = 0.35f;
    public const float TerrainHeightMultiplier = 0.225f; //A multiplier on how much terrain there is until the bottom of the world
    public const float TerrainMinimum = 0.2f;
    public const float NoisePosMult = 0.25f; //Smaller values result in smoother terrain. Larger values result in more varied and jagged terrain
    public const int AirBelowMap = 16; //Eventually, characters will have abilities to place blocks below the map, so the bottom of the map should be air
    //chunk size
    public const int Width = 8;
    public const int Height = 64;
    public const int SeedNoiseModifier = 256;
    private static int Seed { get { return GameStateManager.GenSeed; } }
    public Vector2Int Index { get; set; }
    public int[,,] blocks = new int[Width, Height, Width];
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    // Start is called before the first frame update
    private void Awake()
    {
        meshFilter = this.GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateChunk();
    }
    public void GenerateChunk()
    {
        float stoneLayer = StoneLayer;
        float terrainHeightMult = TerrainHeightMultiplier;
        float terrainMin = TerrainMinimum;
        float noisePosMult = NoisePosMult;
        if(World.WorldType == 1)
        {
            stoneLayer = 0.05f;
            terrainMin = 0.2f;
            terrainHeightMult = 0.1f;
            noisePosMult = 0.1f;
        }
        float seedMod = Seed % SeedNoiseModifier;
        seedMod /= (float)SeedNoiseModifier;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                Vector2 posInNoise = new Vector2(transform.position.x + x, transform.position.z + z) / (float)Width * noisePosMult;
                float noise = Mathf.PerlinNoise(seedMod + posInNoise.x, posInNoise.y) * terrainHeightMult + terrainMin;
                float stoneLayerNoise = Mathf.Sqrt(Mathf.PerlinNoise(posInNoise.y, seedMod + posInNoise.x)) * terrainHeightMult * stoneLayer + terrainMin;
                for (int y = Height - 1; y >= 0; y--)
                {
                    if (y < (Height + AirBelowMap) * noise && y >= AirBelowMap)
                    {
                        if (y < (Height + AirBelowMap) * stoneLayerNoise)
                            blocks[x, y, z] = BlockID.Stone;
                        else
                        {
                            if (World.WorldType == 1)
                            {
                                blocks[x, y, z] = BlockID.Sand;
                            }
                            else
                            {
                                if (blocks[x, y + 1, z] == BlockID.Air)
                                {
                                    blocks[x, y, z] = BlockID.Grass;
                                }
                                else
                                {
                                    blocks[x, y, z] = BlockID.Dirt;
                                }
                            }
                        }
                    }
                    else
                        blocks[x, y, z] = BlockID.Air;
                }
            }
        }
    }
    public bool SolidTile(int x, int y, int z, int myType)
    {
        if (y >= Height || y < 0)
            return false;
        else if (x >= Width || x < 0
            || z >= Width || z < 0)
        {
            int i = 0;
            int j = 0;
            if (x >= Width)
            {
                x = 0;
                i++;
            }
            else if (x < 0)
            {
                x = Width - 1;
                i--;
            }
            if (z >= Width)
            {
                z = 0;
                j++;
            }
            else if(z < 0)
            {
                z = Width - 1;
                j--;
            }
            GameObject ChunkObj = World.Instance.GetChunk(Index.x + i, Index.y + j);
            if (ChunkObj == null)
                return false;
            Chunk chunk = ChunkObj.GetComponent<Chunk>();
            return chunk.SolidTile(x, y, z, myType);
        }
        else
        {
            int blockType = blocks[x, y, z];
            if (blockType != BlockID.Air)
            {
                if(blockType == BlockID.Glass && myType != BlockID.Glass)
                {
                    return false;
                }
                if(blockType != BlockID.Glass && myType == BlockID.Glass)
                {
                    return false;
                }
                if (blockType == BlockID.Leaves)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    // Update is called once per frame
    public void BuildMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int numFaces = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int myType = blocks[x, y, z];
                    if (myType != BlockID.Air)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        if (!SolidTile(x, y + 1, z, myType))
                        {
                            AddVerticies(MeshSide.Top, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).top.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y - 1, z, myType))
                        {
                            AddVerticies(MeshSide.Bottom, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).bottom.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y, z - 1, myType))
                        {
                            AddVerticies(MeshSide.Front, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).front.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x + 1, y, z, myType))
                        {
                            AddVerticies(MeshSide.Right, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).right.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y, z + 1, myType))
                        {
                            AddVerticies(MeshSide.Back, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).back.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x - 1, y, z, myType))
                        {
                            AddVerticies(MeshSide.Left, blockPos, ref vertices);
                            uvs.AddRange(BlockMesh.Get(myType).left.GetUVs());
                            numFaces++;
                        }
                    }
                }
            }
        }

        int tl = vertices.Count - 4 * numFaces;
        for (int i = 0; i < numFaces; i++)
        {
            triangles.AddRange(new int[] { 
                tl + i * 4, 
                tl + i * 4 + 1, 
                tl + i * 4 + 2,
                tl + i * 4, 
                tl + i * 4 + 2, 
                tl + i * 4 + 3});
        }

        if (numFaces <= 0 || vertices.Count <= 0)
        {
            meshFilter.mesh = null;
            meshCollider.sharedMesh = null;
        }
        else
        {
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
    public enum MeshSide
    {
        Front,
        Back,
        Top,
        Bottom,
        Left,
        Right
    }
    public static void AddVerticies(MeshSide side, Vector3 blockPos, ref List<Vector3> vertices)
    {
        if(side == MeshSide.Front)
        {
            vertices.Add(blockPos + new Vector3(0, 0, 0));
            vertices.Add(blockPos + new Vector3(0, 1, 0));
            vertices.Add(blockPos + new Vector3(1, 1, 0));
            vertices.Add(blockPos + new Vector3(1, 0, 0));
        }
        if(side == MeshSide.Back)
        {
            vertices.Add(blockPos + new Vector3(1, 0, 1));
            vertices.Add(blockPos + new Vector3(1, 1, 1));
            vertices.Add(blockPos + new Vector3(0, 1, 1));
            vertices.Add(blockPos + new Vector3(0, 0, 1));
        }
        if (side == MeshSide.Top)
        {
            vertices.Add(blockPos + new Vector3(0, 1, 0));
            vertices.Add(blockPos + new Vector3(0, 1, 1));
            vertices.Add(blockPos + new Vector3(1, 1, 1));
            vertices.Add(blockPos + new Vector3(1, 1, 0));
        }
        if (side == MeshSide.Bottom)
        {
            vertices.Add(blockPos + new Vector3(0, 0, 0));
            vertices.Add(blockPos + new Vector3(1, 0, 0));
            vertices.Add(blockPos + new Vector3(1, 0, 1));
            vertices.Add(blockPos + new Vector3(0, 0, 1));
        }
        if (side == MeshSide.Left)
        {
            vertices.Add(blockPos + new Vector3(0, 0, 1));
            vertices.Add(blockPos + new Vector3(0, 1, 1));
            vertices.Add(blockPos + new Vector3(0, 1, 0));
            vertices.Add(blockPos + new Vector3(0, 0, 0));
        }
        if (side == MeshSide.Right)
        {
            vertices.Add(blockPos + new Vector3(1, 0, 0));
            vertices.Add(blockPos + new Vector3(1, 1, 0));
            vertices.Add(blockPos + new Vector3(1, 1, 1));
            vertices.Add(blockPos + new Vector3(1, 0, 1));
        }
    }
}
