using UnityEngine;

public class BarrierBlock : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    [SerializeField] private GameObject Top, Bottom, Left, Right, Front, Back;
    public bool IgnoreTop, IgnoreBot; //These need to be public as they are modified when this is initialized in other classes
    public void UpdateCollision()
    {
        Vector3 pos = transform.position + new Vector3(0f, 0.5f, 0f);
        if (!IgnoreTop)
            UpdateFaceBacedOnActiveNearbyBlocks(ref Top, pos, y: 1);
        else
            Top.SetActive(false);
        if(!IgnoreBot)
            UpdateFaceBacedOnActiveNearbyBlocks(ref Bottom, pos, y: -1);
        else
            Bottom.SetActive(false);
        UpdateFaceBacedOnActiveNearbyBlocks(ref Left, pos, x: -1);
        UpdateFaceBacedOnActiveNearbyBlocks(ref Right, pos, x: 1);
        UpdateFaceBacedOnActiveNearbyBlocks(ref Front, pos, z: 1);
        UpdateFaceBacedOnActiveNearbyBlocks(ref Back, pos, z: -1);
    }
    public void UpdateFaceBacedOnActiveNearbyBlocks(ref GameObject face, Vector3 pos, float x = 0, float y = 0, float z = 0)
    {
        if (World.Block(pos.x + x, pos.y + y, pos.z + z) == BlockID.Air)
            face.SetActive(false);
        else
            face.SetActive(true);
    }
}
