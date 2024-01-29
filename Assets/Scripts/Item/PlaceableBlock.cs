using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBlock : Item
{
    public int PlaceID { get; protected set; }
    public PlaceableBlock(int BlockID)
    {
        PlaceID = BlockID;
    }
    public override bool ConsumeItem()
    {
        return true;
    }
}
