using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileManager : MonoBehaviour 
{
    private static ProjectileManager storedInstance;
    public static ProjectileData Data;
    public ProjectileData ProjectileData;
    private static ProjectileManager Instance
    {
        get
        {
            if(storedInstance == null) //This mostly exists to reduce errors upon reloading in the editor. Usually, Instance will stay permanently assigned in the game after the first update.
            {
                storedInstance = FindFirstObjectByType<ProjectileManager>();
                ProjectileData.Instance = Data = storedInstance.ProjectileData;
            }
            return storedInstance;
        }
        set
        {
            storedInstance = Instance;
        }
    }
    private void Start()
    {
        Update();
    }
    private void Update()
    {
        Instance = this;
        ProjectileData.Instance = Data = ProjectileData;
    }
    public static GameObject GetProjectile(int ProjectileID)
    {
        return Instance.Get(ProjectileID);
    }
    private GameObject Get(int ProjectileID)
    {
        return ProjectileData.GetProjectile(ProjectileID);
    }
}
