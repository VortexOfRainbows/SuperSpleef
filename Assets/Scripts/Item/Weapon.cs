using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public override void SetDefaults()
    {
        Count = 1;
        MaxCount = 1;
    }
    public override bool OnPrimaryUse()
    {
        return false;
    }
    public override bool OnSecondaryUse()
    {
        return false;
    }
}
