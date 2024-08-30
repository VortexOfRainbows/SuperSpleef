using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBlock : Item ///Team members that contributed to this script: Ian Bunnell
{
    public int PlaceID { get; protected set; }
    public PlaceableBlock(int blockID, int count = 1)
    {
        Count = count;
        PlaceID = blockID;
    }
    public override bool OnSecondaryUse(Player player)
    {
        return true;
    }
    /*public override void SetDefaults() //The default max count is already 9999, so there is no need to set it in here
    {
        MaxCount = 9999;
    } */
    public override bool IsConsumedOnUse(Player player)
    {
        return Main.Mode != GameModeID.Creative; //Don't consume placeable blocks in creative mode
    }
}
