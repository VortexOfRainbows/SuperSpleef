using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[Obsolete] // This script is no longer used.
public class NavMeshUpdater : Projectile ///Team members that contributed to this script: Samuel Gines
{
    public NavMeshSurface navSurface;

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        navSurface.BuildNavMesh();
    }
}