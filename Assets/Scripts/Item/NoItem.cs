using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoItem : Item ///Team members that contributed to this script: Ian Bunnell
{
    public override void SetDefaults()
    {
        Count = 1;
        MaxCount = 1;
    }
    public override bool OnPrimaryUse(Player player)
    {
        return false;
    }
    public override bool OnSecondaryUse(Player player)
    {
        return false;
    }
}
