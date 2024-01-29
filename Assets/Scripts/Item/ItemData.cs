using System;
using UnityEngine;

[Obsolete]
[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ItemData : ScriptableObject
{
    public int PlaceBlock = -1;
}