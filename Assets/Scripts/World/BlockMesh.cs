using System.Collections.Generic;
using UnityEngine;

public class BlockMesh ///Team members that contributed to this script: Ian Bunnell
{
    public static BlockMesh Get(int blockType)
    {
        return tiles[blockType];
    }
    private static Dictionary<int, BlockMesh> tiles = new Dictionary<int, BlockMesh>()
    {
        {BlockID.BlueBricks, new BlockMesh(BlockID.BlueBricks)},
        {BlockID.YellowBricks, new BlockMesh(BlockID.YellowBricks)},
        {BlockID.Wood, new BlockMesh(BlockID.Wood)},
        {BlockID.Leaves, new BlockMesh(BlockID.Leaves)},
        {BlockID.Glass, new BlockMesh(BlockID.Glass)},
        {BlockID.Stone, new BlockMesh(BlockID.Stone)},
        {BlockID.Grass, new BlockMesh(BlockID.Grass)},
        {BlockID.Dirt, new BlockMesh(BlockID.Dirt)},
        {BlockID.Air, new BlockMesh(BlockID.Air)},
    };
    private int Type;
    public BlockFace top, left, right, front, back, bottom;
    public List<BlockFace> UniqueFaces { get; private set; }
    private void SetFaceArray()
    {
        BlockFace[] faces = new BlockFace[] { top, left, right, front, back, bottom };
        UniqueFaces = new List<BlockFace>();
        foreach (BlockFace face in faces)
        {
            if (!UniqueFaces.Contains(face))
            {
                UniqueFaces.Add(face);
            }
        }
    }
    private BlockMesh(int type)
    {
        Type = type;
        SetSprites();
    }
    private void SetSprites() //Sets the sprites for the dictionary, so other classes can easily access block uv maps (sprites)
    {
        if (Type == BlockID.Air)
        {
            SetAllFaces(Tile.Air);
        }
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
        if (Type == BlockID.Glass)
        {
            SetAllFaces(Tile.Glass);
        }
        if (Type == BlockID.Stone)
        {
            SetAllFaces(Tile.Stone);
        }
        if (Type == BlockID.Wood)
        {
            top = bottom = BlockFace.FaceSprite(Tile.Log);
            right = front = back = left = BlockFace.FaceSprite(Tile.LogSide);
        }
        if (Type == BlockID.Leaves)
        {
            SetAllFaces(Tile.Leaves);
        }
        if (Type == BlockID.BlueBricks)
        {
            SetAllFaces(Tile.BlueBricks);
        }
        if (Type == BlockID.YellowBricks)
        {
            SetAllFaces(Tile.YellowBricks);
        }
        SetFaceArray();
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