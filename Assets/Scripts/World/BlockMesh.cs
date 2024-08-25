using System.Collections.Generic;
using UnityEngine;

public class BlockMesh ///Team members that contributed to this script: Ian Bunnell
{
    public static BlockMesh Get(int blockType)
    {
        return BlockFaceIDs[blockType];
    }
    private static Dictionary<int, BlockMesh> BlockFaceIDs = InitDictionary();
    private static Dictionary<int, BlockMesh> InitDictionary()
    {
        Dictionary<int, BlockMesh> dict = new Dictionary<int, BlockMesh>();
        for(int i = 0; i < BlockID.Max; i++)
            dict.Add(i, new BlockMesh(i));
        return dict;
    }
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
            SetAllFaces(BlockFaceID.Air);
        }
        if (Type == BlockID.Dirt)
        {
            SetAllFaces(BlockFaceID.Dirt);
        }
        if (Type == BlockID.Grass)
        {
            top = BlockFace.FaceSprite(BlockFaceID.Grass);
            right = front = back = left = BlockFace.FaceSprite(BlockFaceID.GrassSide);
            bottom = BlockFace.FaceSprite(BlockFaceID.Dirt);
        }
        if (Type == BlockID.Glass)
        {
            SetAllFaces(BlockFaceID.Glass);
        }
        if (Type == BlockID.Stone)
        {
            SetAllFaces(BlockFaceID.Stone);
        }
        if (Type == BlockID.Wood)
        {
            top = bottom = BlockFace.FaceSprite(BlockFaceID.Log);
            right = front = back = left = BlockFace.FaceSprite(BlockFaceID.LogSide);
        }
        if (Type == BlockID.Leaves)
        {
            SetAllFaces(BlockFaceID.Leaves);
        }
        if (Type == BlockID.BlueBricks)
        {
            SetAllFaces(BlockFaceID.BlueBricks);
        }
        if (Type == BlockID.YellowBricks)
        {
            SetAllFaces(BlockFaceID.YellowBricks);
        }
        if (Type == BlockID.Sand)
        {
            SetAllFaces(BlockFaceID.Sand);
        }
        if (Type == BlockID.Cactus)
        {
            top = bottom = BlockFace.FaceSprite(BlockFaceID.Cactus);
            right = front = back = left = BlockFace.FaceSprite(BlockFaceID.CactusSide);
        }
        if (Type == BlockID.Eye)
        {
            top = BlockFace.FaceSprite(BlockFaceID.EyeTop);
            right = front = back = left = BlockFace.FaceSprite(BlockFaceID.EyeSide);
            bottom = BlockFace.FaceSprite(BlockFaceID.EyeBottom);
        }
        SetFaceArray();
    }
    private void SetAllFaces(int BlockFaceID)
    {
        top = BlockFace.FaceSprite(BlockFaceID);
        left = top;
        right = top;
        front = top;
        back = top;
        bottom = top;
    }
}