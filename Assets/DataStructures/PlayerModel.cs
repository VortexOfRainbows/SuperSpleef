using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class PlayerModel : ScriptableObject
{
    public Mesh HeadMesh, BodyMesh, LeftLegMesh, RightLegMesh, LeftArmMesh, RightArmMesh;
    private const int SheetSizeX = 64;
    private const int SheetSizeY = 48;
    public void Setup()
    {
        SetUpUVs();
    }
    private Vector2[] CreateUVsOutOfCoordinates(Rect rect)
    {
        float xPos = rect.x, yPos = rect.y, width = rect.width, height = rect.height;
        yPos = SheetSizeY - yPos - height; //This helps anchor the sprite at the top left instead of bottom left, which is easier for my purposes.
        float smallStepX = 0, smallStepY = 0;
        float xLeft = xPos / SheetSizeX + smallStepX;
        float xRight = (xPos + width) / SheetSizeX - smallStepX;
        float yBottom = yPos / SheetSizeY + smallStepY;
        float yTop = (yPos + height) / SheetSizeY - smallStepY;
        Vector2[] uv = new Vector2[]
        {
            new Vector2(xLeft, yBottom),
            new Vector2(xLeft, yTop),
            new Vector2(xRight, yTop),
            new Vector2(xRight, yBottom),
        };
        return uv;
    }
    private void SetUpUVs()
    {
        /// The order is Top, Bottom, Back, Left (Right from player POV), Front, Right (Left from player POV)
        SetUpMesh(ref HeadMesh, "HeadMesh", new Rect[] { 
            new Rect(8, 0, 8, 8), 
            new Rect(16, 0, 8, 8),
            new Rect(24, 8, 8, 8),
            new Rect(0, 8, 8, 8),
            new Rect(8, 8, 8, 8),
            new Rect(16, 8, 8, 8)
        });
        SetUpMesh(ref BodyMesh, "BodyMesh", new Rect[] {
            new Rect(20, 16, 8, 4),
            new Rect(28, 16, 8, 4),
            new Rect(32, 20, 8, 12),
            new Rect(16, 20, 4, 12),
            new Rect(20, 20, 8, 12),
            new Rect(28, 20, 4, 12)
        });
        SetUpMesh(ref RightLegMesh, "RightLegMesh", new Rect[] {
            new Rect(4, 16, 4, 4),
            new Rect(8, 16, 4, 4),
            new Rect(12, 20, 4, 12),
            new Rect(0, 20, 4, 12),
            new Rect(4, 20, 4, 12),
            new Rect(8, 20, 4, 12)
        });
        SetUpMesh(ref LeftLegMesh, "LeftLegMesh", new Rect[] {
            new Rect(4, 32, 4, 4),
            new Rect(8, 32, 4, 4),
            new Rect(12, 36, 4, 12),
            new Rect(0, 36, 4, 12),
            new Rect(4, 36, 4, 12),
            new Rect(8, 36, 4, 12)
        });
        SetUpMesh(ref RightArmMesh, "RightArmMesh", new Rect[] {
            new Rect(44, 16, 4, 4),
            new Rect(48, 16, 4, 4),
            new Rect(52, 20, 4, 12),
            new Rect(40, 20, 4, 12),
            new Rect(44, 20, 4, 12),
            new Rect(48, 20, 4, 12)
        });
        SetUpMesh(ref LeftArmMesh, "LeftArmMesh", new Rect[] {
            new Rect(44, 32, 4, 4),
            new Rect(48, 32, 4, 4),
            new Rect(52, 36, 4, 12),
            new Rect(40, 36, 4, 12),
            new Rect(44, 36, 4, 12),
            new Rect(48, 36, 4, 12)
        });
    }
    private void SetUpMesh(ref Mesh mesh, string fileName, Rect[] rects)
    {
        mesh = Utils.GenerateCubeMesh();
        List<Vector2> uvs = new List<Vector2>();

        /// The order is Top, Bottom, Back, Left (Right from player POV), Front, Right (Left from player POV)
        for(int i = 0; i < 6; i++)
            uvs.AddRange(CreateUVsOutOfCoordinates(rects[i]));

        mesh.uv = uvs.ToArray();
        
        /// This is to make sure that the UVs don't have to be recalculated every time.
        /// However, this must be commented out for the build version
        /// When UVs need to be changed, uncomment out this section
        
        //AssetDatabase.CreateAsset(mesh, "Assets/DataStructures/" + fileName + ".asset");
        //AssetDatabase.SaveAssets();
    }
}