using System.Collections.Generic;
using UnityEngine;

public class BlockFace ///Team members that contributed to this script: Ian Bunnell
{
    public const float SpriteSize = 16;
    public const float TotalSpritesX = 16;
    public const float TotalSpritesY = 16;
    public const float Padding = 0;
    public Vector2Int UVPos { get; private set; }
    public static BlockFace FaceSprite(int tile)
    {
        return tiles[tile];
    }
    private static Dictionary<int, BlockFace> tiles = new Dictionary<int, BlockFace>()
    {
        {BlockFaceID.Grass, new BlockFace(0,0)},
        {BlockFaceID.GrassSide, new BlockFace(0,1)},
        {BlockFaceID.Dirt, new BlockFace(0,2)},
        {BlockFaceID.Glass, new BlockFace(0,3)},
        {BlockFaceID.Stone, new BlockFace(0,4)},
        {BlockFaceID.LogSide, new BlockFace(0,5)},
        {BlockFaceID.Log, new BlockFace(0,6)},
        {BlockFaceID.Leaves, new BlockFace(0,7)},
        {BlockFaceID.YellowBricks, new BlockFace(0,8)},
        {BlockFaceID.BlueBricks, new BlockFace(0,9)},
        {BlockFaceID.Air, new BlockFace(1,0)},
        {BlockFaceID.Sand, new BlockFace(0,10)},
        {BlockFaceID.CactusSide, new BlockFace(0,11)},
        {BlockFaceID.Cactus, new BlockFace(0,12)},
        {BlockFaceID.EyeTop, new BlockFace(0,13)},
        {BlockFaceID.EyeSide, new BlockFace(0,14)},
        {BlockFaceID.EyeBottom, new BlockFace(0,15)},
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