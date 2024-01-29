using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public static Item Empty { get; private set; } = new NoItem();
    public const int DefaultMaxCount = 9999; //The default max count will be 9999.
    public static Dictionary<int, Item> ItemType;
    public int Count { get; protected set; }
    public int MaxCount { get; protected set; }
    public Item(int count = 1)
    {
        Count = count;
        MaxCount = DefaultMaxCount;
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
    public virtual bool IsConsumedOnUse()
    {
        return false;
    }
    public void ModifyCount(int change)
    {
        Count += change;
        if (Count > MaxCount)
            Count = MaxCount;
        if(Count <= 0)
        {
            Count = 0;
        }
    }
}
