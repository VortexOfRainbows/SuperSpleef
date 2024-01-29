using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public static Dictionary<int, Item> ItemType;
    public Item()
    {
        SetDefaults();
    }
    /// <summary>
    /// Run once when an item is initialized
    /// </summary>
    public virtual void SetDefaults()
    {

    }
    /// <summary>
    /// Run when a held item is left clicked.
    /// Return true on a successful use
    /// Defaults to true
    /// </summary>
    public virtual bool OnPrimaryUse()
    {
        return true;
    }
    /// <summary>
    /// Run when a held item is right clicked.
    /// Return true on a successful use
    /// Defaults to true
    /// </summary>
    public virtual bool OnSecondaryUse()
    {
        return true;
    }
    /// <summary>
    /// Whether or not an item should be consumed upon successfully being used.
    /// Defaults to false
    /// </summary>
    /// <returns></returns>
    public virtual bool ConsumeItem()
    {
        return false;
    }
}
