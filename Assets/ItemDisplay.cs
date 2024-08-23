using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    private Item prevItem;
    public Item item;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public bool HasSwappedModels = false;
    public void SetModelToItemModel()
    {
        bool modelIsBlock = true; //For now, all models are blocks
        if (modelIsBlock)
        {
            int blockType = BlockID.Air;
            if (item is PlaceableBlock pb)
            {
                blockType = pb.PlaceID;
            }
            ///TBD: Set to correct material here too
            SetModelToBlock(blockType);

            ///Each model will probably need to specify draw related parameters like this
            transform.localPosition = new Vector3(0, -0.125f, 0.5f);
            transform.localEulerAngles = new Vector3(0, 45, 0);
            transform.localScale = Vector3.one * 1.25f;
        }
    }
    public void SetModelToBlock(int BlockID)
    {
        if (!HasSwappedModels)
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
        HasSwappedModels = true;
    }
    private void Update()
    {
        if(prevItem != item)
        {
            HasSwappedModels = false;
            prevItem = item;
        }
        SetModelToItemModel();
        if(item != null && item is not NoItem)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
