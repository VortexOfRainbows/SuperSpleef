using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils ///Team members that contributed to this script: Ian Bunnell
{
    /// <summary>
    /// A CubeMesh that works for the UV maps of tiles in the tile atlas
    /// Automatically rebaked when assigned to a mesh (so you can modify the mesh elsewhere without changing this helper function)
    /// </summary>
    public static Mesh CubeMesh 
    { 
        get
        {
            if(StoredMesh == null)
            {
                StoredMesh = GenerateCubeMesh();
            }
            return StoredMesh;
        }
        set
        {
            StoredMesh = value;
        }
    }
    private static Mesh StoredMesh = null;
    /// <summary>
    /// Rotates a vector based on the radians inputted. Useful for created circular effects
    /// </summary>
    /// <param name="spinningpoint"></param>
    /// <param name="radians"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    public static Vector2 RotatedBy(this Vector2 spinningpoint, float radians, Vector2 center = default(Vector2))
    {
        float xMult = (float)MathF.Cos(radians);
        float yMult = (float)MathF.Sin(radians);
        Vector2 vector = spinningpoint - center;
        Vector2 result = center;
        result.x += vector.x * xMult - vector.y * yMult;
        result.y += vector.x * yMult + vector.y * xMult;
        return result;
    }
    /// <summary>
    /// Converts the vector to the rotation in radians which would make the vector (magnitude, 0) become the vector.
    /// Basically runs arctan on the vector. y/x
    /// </summary>
    /// <param name="directionVector"></param>
    /// <returns></returns>
    public static float ToRotation(this Vector2 directionVector)
    {
        return Mathf.Atan2(directionVector.y, directionVector.x);
    }
    /// <summary>
    /// Converts a rotation to a quaternion rotated on the z axis.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Quaternion ToQuaternion(this float rotation)
    {
        Quaternion relativeRotation = Quaternion.AngleAxis(rotation * Mathf.Rad2Deg, new Vector3(0, 0, 1));
        return relativeRotation;
    }
    /// <summary>
    /// Wraps an angle to an equivalent angle between -Pi and Pi
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float WrapAngle(this float x)
    {
        x = (x + Mathf.PI) % (2 * Mathf.PI);
        if (x < 0)
            x += Mathf.PI * 2;
        return x - Mathf.PI;
    }
    /// <summary>
    /// Gets the position of the mouse in the world. Not sure if this works in 3D since this helper was originally written for 2D.
    /// </summary>
    /// <returns></returns>
    public static Vector2 MouseWorld()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mPos;
    }
    /// <summary>
    /// Converts a string from something like this: "StringHereIs" to "String Here Is"
    /// Adjacent capital letters are considered part of the same string: "SUPERSpeed" to "SUPERSpeed"
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string AddSpaceBetweenCaps(this string str)
    {
        string construct = string.Empty;
        for (int i = 0; i < str.Length - 1; i++)
        {
            char first = str[i];
            char second = str[i + 1];
            construct += first;
            if (Char.IsLower(first) && Char.IsUpper(second))
            {
                construct += " ";
            }
        }
        construct += str[str.Length - 1];
        return construct;
    }
    private static Mesh GenerateCubeMesh() //Generates a basic cube mesh for usage in various areas
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 center = Vector3.one * -0.5f;
        Chunk.AddVerticies(Chunk.MeshSide.Top, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Bottom, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Front, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Right, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Back, center, ref vertices);
        Chunk.AddVerticies(Chunk.MeshSide.Left, center, ref vertices);
        for (int i = 0; i < 6; i++)
        {
            triangles.AddRange(new int[] {
                i * 4,
                i * 4 + 1,
                i * 4 + 2,
                i * 4,
                i * 4 + 2,
                i * 4 + 3});
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
