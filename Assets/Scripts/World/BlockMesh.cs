using System.Collections.Generic;
using UnityEngine;

public class BlockMesh
{
    public static BlockMesh Get(int blockType)
    {
        return tiles[blockType];
    }
    private static Dictionary<int, BlockMesh> tiles = new Dictionary<int, BlockMesh>()
    {
        {BlockID.Grass, new BlockMesh(BlockID.Grass)},
        {BlockID.Dirt, new BlockMesh(BlockID.Dirt)},
    };
    private int Type;
    public BlockFace top, left, right, front, back, bottom;
    private BlockMesh(int type)
    {
        Type = type;
        SetSprites();
    }
    private void SetSprites()
    {
        if (Type == BlockID.Dirt)
        {
            SetAllFaces(Tile.Dirt);
        }
        if (Type == BlockID.Grass)
        {
            top = BlockFace.FaceSprite(Tile.Grass);
            right = front = back = left = BlockFace.FaceSprite(Tile.GrassSide);
            bottom = BlockFace.FaceSprite(Tile.Dirt);
        }
    }
    private void SetAllFaces(Tile tile)
    {
        top = BlockFace.FaceSprite(tile);
        left = top;
        right = top;
        front = top;
        back = top;
        bottom = top;
    }
}