using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshRendering : MonoBehaviour
{
    // Start is called before the first frame update

    public NavMeshSurface/*[]*/ navSurface;

    void Start()
    {

        //navSurfaces = ; 

        //for (int i = 0; i < navSurfaces.Length; i++)
        //{
        //    navSurfaces[i].BuildNavMesh();
        //}

        navSurface.BuildNavMesh();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
