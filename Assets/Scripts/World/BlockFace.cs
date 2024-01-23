using System.Collections.Generic;
using UnityEngine;

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