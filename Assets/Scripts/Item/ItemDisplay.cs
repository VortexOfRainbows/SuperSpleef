using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    private Item prevItem;
    public Item item;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public bool HasSwappedModels = false;
    public bool FirstPerson;
    private GameObject ExternalModel;
    public void SetModelToItemModel()
    {
        if(!HasSwappedModels)
        {
            if(ExternalModel != null)
            {
                Destroy(ExternalModel);
                ExternalModel = null;
            }
        }
        bool modelIsBlock = true; 
        if (item == null || item is NoItem)
        {
            meshRenderer.enabled = false;
            modelIsBlock = false;
        }
        if (item is Drill)
        {
            if(!HasSwappedModels)
                ExternalModel = Instantiate(Main.ItemData.Hammer, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            if (FirstPerson)
            {
                transform.localPosition = new Vector3(0f, -.5f, -2.25f);
                transform.localEulerAngles = new Vector3(90, 0, 0);
                transform.localScale = Vector3.one * 3f;
            }
            else
            {
                transform.localPosition = new Vector3(0, -.85f, -2.5f);
                transform.localEulerAngles = new Vector3(100, 180, 180);
                transform.localScale = Vector3.one * 3.75f;
            }
            meshRenderer.enabled = false;
            modelIsBlock = false;
        }
        if (modelIsBlock)
        {
            meshRenderer.enabled = true;
            int blockType = BlockID.Air;
            if (item is LaserCannon)
                blockType = BlockID.Eye;
            if (item is PlaceableBlock pb)
            {
                blockType = pb.PlaceID;
            }
            ///TBD: Set to correct material here too
           
            if(!HasSwappedModels)
                SetModelToBlock(blockType);

            ///Each model will probably need to specify draw related parameters like this
            if(FirstPerson)
            {
                transform.localPosition = new Vector3(0, -0.25f, 0.5f);
                //transform.localEulerAngles = new Vector3(0, 27, 0);
                if(item is LaserCannon)
                    transform.localEulerAngles = new Vector3(180, 27, 0);
                else 
                    transform.localEulerAngles = new Vector3(90, 27, 0);
                transform.localScale = Vector3.one * 1.45f;
            }
            else
            {
                transform.localPosition = new Vector3(0, -0.125f, 0.5f);
                if (item is LaserCannon)
                    transform.localEulerAngles = new Vector3(180, 45, 0);
                else
                    transform.localEulerAngles = new Vector3(0, 45, 0);
                transform.localScale = Vector3.one * 1.25f;
            }
        }
        HasSwappedModels = true;
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
    }
    private void Update()
    {
        if(prevItem != item)
        {
            HasSwappedModels = false;
            prevItem = item;
        }
        SetModelToItemModel();
        if(ExternalModel != null)
        {
            ExternalModel.gameObject.layer = gameObject.layer;
            foreach(Transform child in ExternalModel.GetComponentInChildren<Transform>())
            {
                child.gameObject.layer = gameObject.layer;
            }
        }
    }
}
