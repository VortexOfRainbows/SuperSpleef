using System.Collections.Generic;
using UnityEngine;

public static class BlockID
{
    public static int Air = 0;
    public static int Dirt = 1;
    public static int Grass = 2;
}
public class Block
{
    public static Block GetBlock(int blockType)
    {
        return tiles[blockType];
    }
    private static Dictionary<int, Block> tiles = new Dictionary<int, Block>()
    {
        {BlockID.Grass, new Block(BlockID.Grass)},
        {BlockID.Dirt, new Block(BlockID.Dirt)},
    };
    public int Type;
    public BlockFace top, left, right, front, back, bottom;
    private Block(int type)
    {
        Type = type;
        SetSprites();
    }
    private void SetSprites()
    {
        if (Type == BlockID.Dirt)
        {
            top = BlockFace.FaceSprite(Tile.Dirt);
            SetSpritesToTop();
        }
        if (Type == BlockID.Grass)
        {
            top = BlockFace.FaceSprite(Tile.Grass);
            right = front = back = left = BlockFace.FaceSprite(Tile.GrassSide);
            bottom = BlockFace.FaceSprite(Tile.Dirt);
        }
    }
    private void SetSpritesToTop()
    {
        left = top;
        right = top;
        front = top;
        back = top;
        bottom = top;
    }
}

public class BlockFace
{
    public const float SpriteSize = 16;
    public const float TotalSpritesX = 16;
    public const float TotalSpritesY = 16;
    public const float Padding = 0;
    public static BlockFace FaceSprite(Tile tile)
    {
        return tiles[tile];
    }
    private static Dictionary<Tile, BlockFace> tiles = new Dictionary<Tile, BlockFace>()
    {
        {Tile.Grass, new BlockFace(0,0)},
        {Tile.GrassSide, new BlockFace(0,1)},
        {Tile.Dirt, new BlockFace(0,2)},
    };
    private readonly Vector2[] uvs;
    private BlockFace(int xPos, int yPos) //yPos is how many tiles up it is from bottom. x is how many tiles to the right
    {
        float smallStepX = 1f / 4 / TotalSpritesX / SpriteSize;
        float smallStepY = 1f / 4 / TotalSpritesY / SpriteSize;
        float xLeft = xPos / TotalSpritesX + smallStepX;
        float xRight = (xPos + 1) / TotalSpritesX - smallStepX;
        float yBottom = yPos / TotalSpritesY + smallStepY;
        float yTop = (yPos + 1) / TotalSpritesY - smallStepY;
        uvs = new Vector2[]
        {
            new Vector2(xLeft, yBottom),
            new Vector2(xLeft, yTop),
            new Vector2(xRight, yTop),
            new Vector2(xRight, yBottom),
        };
    }
    public Vector2[] GetUVs()
    {
        return uvs;
    }
}
public enum Tile { Dirt, Grass, GrassSide }