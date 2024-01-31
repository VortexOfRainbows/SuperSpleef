using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Purchasing;
using UnityEngine;

public static class Utils
{
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
    public static Quaternion ToQuaternion(this float rotation)
    {
        Quaternion relativeRotation = Quaternion.AngleAxis(rotation * Mathf.Rad2Deg, new Vector3(0, 0, 1));
        return relativeRotation;
    }
    public static float WrapAngle(this float x)
    {
        x = (x + Mathf.PI) % (2 * Mathf.PI);
        if (x < 0)
            x += Mathf.PI * 2;
        return x - Mathf.PI;
    }
    public static Vector2 MouseWorld()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mPos;
    }
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
    private static Mesh GenerateCubeMesh()
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
