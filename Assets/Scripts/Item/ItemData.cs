using System;
using UnityEngine;

[Obsolete] //I'm not sure I will use ITEMDATA at all in this project...
[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ItemData : ScriptableObject
{
    public int PlaceBlock = -1;
}