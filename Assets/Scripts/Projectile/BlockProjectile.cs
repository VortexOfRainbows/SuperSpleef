using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProjectile : Projectile
{
    [SerializeField] private MeshFilter meshFilter;
    private int MyBlockID;
    public override void OnSpawn()
    {
        MyBlockID = BlockID.Stone;
        if (Random.value < 0.5f)
            MyBlockID = BlockID.Dirt;
        SetModelToBlock(MyBlockID);
    }
    public void SetModelToBlock(int BlockID)
    {
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
    }
    public override void OnKill(bool OutBoundDeath)
    {
        if(!OutBoundDeath)
        {
            Vector3 HitPoint = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, Mathf.FloorToInt(transform.position.z) + 0.5f);
            World.SetBlock(HitPoint, MyBlockID);
        }
    }
}
