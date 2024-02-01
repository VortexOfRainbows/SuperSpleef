using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InventoryModel : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    private bool MeshIsCurrentlyBlock = false;
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
        if(!MeshIsCurrentlyBlock)
            meshFilter.mesh = Utils.CubeMesh;
        int blockId = BlockID;
        List<Vector2> uvs = new List<Vector2>();
        uvs.AddRange(BlockMesh.Get(blockId).top.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).bottom.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).front.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).right.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).back.GetUVs());
        uvs.AddRange(BlockMesh.Get(blockId).left.GetUVs());
        meshFilter.mesh.uv = uvs.ToArray();
        MeshIsCurrentlyBlock = true;
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
