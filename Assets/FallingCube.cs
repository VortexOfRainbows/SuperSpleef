using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

///Team members that contributed to this script: Ian Bunnell
public class FallingCube : MonoBehaviour
{
    public static float AdditionalSpeedBasedOnBlocksBrokenInARow()
    {
        return 1 + Mathf.Log10(1 + BlocksBrokenInARow);
    }
    public static int BlocksBrokenInARow { get; private set; } = 0;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private float rotationSpeedMultiplier;
    private Vector3 rotations;
    private int MyType;
    private float fallingSpeed;
    private float DeathY = 0;
    public void SetDeathScreenBound(float bound)
    {
        DeathY = bound;
    }
    public void Awake()
    {
        fallingSpeed = Random.Range(0.5f, 1.5f) * AdditionalSpeedBasedOnBlocksBrokenInARow();
        MyType = Random.Range(BlockID.Dirt, BlockID.Max);
        SetModelToBlock(MyType);
        rotations = transform.localEulerAngles;
        rotationSpeed = Utils.randVector3Circular(1, 1, 1, true) * rotationSpeedMultiplier;
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
    private Vector3 rotationSpeed;
    private void Update()
    {
        rotations += rotationSpeed * Time.deltaTime;
        Vector3 euler = new Vector3(rotations.x, rotations.y, rotations.z);
        transform.localEulerAngles = euler;
        transform.position += Vector3.down * Time.deltaTime * fallingSpeed;
        if(transform.position.y < DeathY)
        {
            NaturalDeath = true;
            Destroy(gameObject);
        }
    }
    private bool NaturalDeath = false;
    private void OnDestroy()
    {
        if(NaturalDeath)
        {
            BlocksBrokenInARow = 0;
            return;
        }
    }
    public void DestroyEffects()
    {
        BlocksBrokenInARow++;
        World.GenerateBlockBreakingParticles(transform.position, MyType, null);
        World.GenerateBlockBreakSound(transform.position, MyType);
        Destroy(gameObject);
    }
}
