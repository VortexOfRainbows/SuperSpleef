using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InventoryModel : MonoBehaviour
{
    private static Mesh GenerateCubeMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 center = Vector3.one * -0.5f;
        Chunk.AddVerticies(Chunk.MeshSide.Top, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Bottom, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Front, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Right, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Back, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Left, center, ref vertices);
        for (int i = 0; i < 6; i++)
        {
            triangles.AddRange(new int[] {
                i * 4,
                i * 4 + 1,
                i * 4 + 2,
                i * 4,
                i * 4 + 2,
                i * 4 + 3});
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
    [SerializeField] private ItemSlot parentSlot;
    [SerializeField] private MeshFilter meshFilter;
    private Vector3 rotations;
    public void Start()
    {
        SetModelToBlock(BlockID.Air);
        rotations = transform.localEulerAngles;
        int index = parentSlot.ItemIndex;
        rotationSpeedX = (10 - index % 10) * Mathf.Deg2Rad * XZRotationSpeedMult;
        rotationSpeedZ = (1 + index % 10) * Mathf.Deg2Rad * XZRotationSpeedMult;
    }
    public void SetModelToBlock(int BlockID)
    {
        meshFilter.mesh = GenerateCubeMesh();
        int blockId = BlockID;
        List<Vector2> uvs = new List<Vector2>();
        uvs.AddRange(BlockMesh.Get(blockId).top.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).bottom.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).front.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).right.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).back.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).left.GetUVs());
        meshFilter.mesh.uv = uvs.ToArray();
    }
    [SerializeField] private float MaxVariationTiltXZ = 9.5f;
    [SerializeField] private float XZRotationSpeedMult = 0.05f;
    [SerializeField] private float rotationSpeedY = 0.05f;
    // Update is called once per frame
    private float rotationSpeedX;
    private float rotationSpeedZ;
    void Update()
    {
        rotations.x += rotationSpeedX;
        rotations.y += rotationSpeedY;
        rotations.z += rotationSpeedZ;
        Vector3 euler = new Vector3(Mathf.Sin(rotations.x) * MaxVariationTiltXZ, rotations.y, Mathf.Cos(rotations.z) * MaxVariationTiltXZ);
        transform.localEulerAngles = euler;
    }
}
