using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public Vector2Int Index { get; set; }
    //chunk size
    public const int Width = 8;
    public const int Height = 64;
    public int[,,] blocks = new int[Width, Height, Width];
    private MeshFilter meshFilter;
    // Start is called before the first frame update
    private void Awake()
    {
        meshFilter = this.GetComponent<MeshFilter>();
        GenerateChunk();
    }
    public void GenerateChunk()
    {        
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                float noise = Mathf.Sqrt(Mathf.PerlinNoise((transform.position.x + x) / Width, (transform.position.z + z) / Width));
                for (int y = 0; y < Height; y++)
                {
                    if (y < Height * noise)
                        blocks[x, y, z] = 2;
                    else
                        blocks[x, y, z] = 0;
                }
            }
        }
    }
    public bool SolidTile(int x, int y, int z)
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
            return chunk.SolidTile(x, y, z);
        }
        else
        {
            int blockType = blocks[x, y, z];
            if (blockType != 0)
                return true;
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
                    if (blocks[x,y,z] != 0)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        if (!SolidTile(x, y + 1, z))
                        {
                            AddVerticies(MeshSide.Top, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).top.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y - 1, z))
                        {
                            AddVerticies(MeshSide.Bottom, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).bottom.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y, z - 1))
                        {
                            AddVerticies(MeshSide.Front, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).front.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x + 1, y, z))
                        {
                            AddVerticies(MeshSide.Right, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).right.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x, y, z + 1))
                        {
                            AddVerticies(MeshSide.Back, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).back.GetUVs());
                            numFaces++;
                        }
                        if (!SolidTile(x - 1, y, z))
                        {
                            AddVerticies(MeshSide.Left, blockPos, ref vertices);
                            uvs.AddRange(Block.GetBlock(blocks[x, y, z]).left.GetUVs());
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
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    private enum MeshSide
    {
        Front,
        Back,
        Top,
        Bottom,
        Left,
        Right
    }
    private void AddVerticies(MeshSide side, Vector3 blockPos, ref List<Vector3> vertices)
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
