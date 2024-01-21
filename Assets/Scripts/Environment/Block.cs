using System.Collections.Generic;
using UnityEngine;

public static class BlockType
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
        {BlockType.Grass, new Block(BlockType.Dirt)},
        {BlockType.Dirt, new Block(BlockType.Grass)},
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
        if(Type == BlockType.Dirt)
        {
            top = BlockFace.FaceSprite(Tile.Dirt);
            SetSpritesToTop();
        }
        if (Type == BlockType.Grass)
        {
            top = BlockFace.FaceSprite(Tile.Grass);
            SetSpritesToTop();
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
    public static BlockFace FaceSprite(Tile tile)
    {
        return tiles[tile];
    }
    private static Dictionary<Tile, BlockFace> tiles = new Dictionary<Tile, BlockFace>()
    {
        {Tile.Grass, new BlockFace(0,1)},
        {Tile.Dirt, new BlockFace(0,0)},
    };
    private Vector2[] uvs;
    private BlockFace(int xPos, int yPos)
    {
        uvs = new Vector2[]
        {
            new Vector2(xPos/16f + .001f, yPos/16f + .001f),
            new Vector2(xPos/16f+ .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, yPos/16f+ .001f),
        };
    }
    public Vector2[] GetUVs()
    {
        return uvs;
    }
}
public enum Tile { Dirt, Grass }