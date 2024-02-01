using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileManager : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    private static ProjectileManager Instance
    {
        get
        {
            if(storedInstance == null) //This mostly exists to reduce errors upon reloading in the editor. Usually, Instance will stay permanently assigned in the game after the first update.
            {
                storedInstance = FindFirstObjectByType<ProjectileManager>(); 
            }
            return storedInstance;
        }
        set
        {
            storedInstance = Instance;
        }
    }
    private static ProjectileManager storedInstance;
    [SerializeField] private GameObject[] ProjectilePrefabs;
    private void Start()
    {
        Update();
    }
    private void Update()
    {
        Instance = this;
    }
    /// <summary>
    /// Returns the prefab associated with the ProjectileType inputted into the function
    /// If multiple prefabs utilize the same ProjectileType, uses the first one found
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static GameObject GetProjectile<T>()
    {
        return Instance.Get<T>();
    }
    private GameObject Get<T>()
    {
        foreach(GameObject prefab in ProjectilePrefabs)
        {
            if (prefab.GetComponent<T>() != null)
                return prefab;
        }
        return null;
    }
    public static GameObject GetProjectile(int ProjectileID)
    {
        return Instance.Get(ProjectileID);
    }
    private GameObject Get(int ProjectileID)
    {
        return ProjectilePrefabs[ProjectileID];
    }
}
