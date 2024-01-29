using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBlock : Item
{
    public int PlaceID { get; protected set; }
    public PlaceableBlock(int blockID, int count = 1)
    {
        Count = count;
        PlaceID = blockID;
    }
    /*public override void SetDefaults() //The default max count is already 9999, so there is no need to set it in here
    {
        MaxCount = 9999;
    } */
    public override bool IsConsumedOnUse()
    {
        return true;
    }
}
