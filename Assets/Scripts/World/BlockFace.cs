using System.Collections.Generic;
using UnityEngine;

public class BlockFace ///Team members that contributed to this script: Ian Bunnell
{
    public const float SpriteSize = 16;
    public const float TotalSpritesX = 16;
    public const float TotalSpritesY = 16;
    public const float Padding = 0;
    public Vector2Int UVPos { get; private set; }
    public static BlockFace FaceSprite(Tile tile)
    {
        return tiles[tile];
    }
    private static Dictionary<Tile, BlockFace> tiles = new Dictionary<Tile, BlockFace>()
    {
        {Tile.Grass, new BlockFace(0,0)},
        {Tile.GrassSide, new BlockFace(0,1)},
        {Tile.Dirt, new BlockFace(0,2)},
        {Tile.Glass, new BlockFace(0,3)},
        {Tile.Stone, new BlockFace(0,4)},
        {Tile.LogSide, new BlockFace(0,5)},
        {Tile.Log, new BlockFace(0,6)},
        {Tile.Leaves, new BlockFace(0,7)},
        {Tile.Air, new BlockFace(1,0)},
    };
    private readonly Vector2[] uvs;
    private BlockFace(int xPos, int yPos) //yPos is how many tiles up it is from bottom. x is how many tiles to the right
    {
        UVPos = new Vector2Int(xPos, yPos);
        //This does the math for fetching the position of a block sprite on the tile atlas
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
public enum Tile { Dirt, Grass, GrassSide, Glass, Stone, LogSide, Log, Leaves, Air }