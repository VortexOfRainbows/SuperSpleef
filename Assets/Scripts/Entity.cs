using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour //A monobehavior class that possesses an inventory
{
    //This class will be used for Enemies, Players, Chests, etc.
    public Inventory Inventory { get; protected set; } 
}
