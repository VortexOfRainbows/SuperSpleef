using System.Collections.Generic;

///Team members that contributed to this script: Ian Bunnell
public abstract class Item //This might not need to be a monobehavior
{
    public const int DefaultItemFirerate = 20;
    public static Item Empty => new NoItem();
    public const int DefaultMaxCount = 9999; //The default max count will be 9999.
    public static Dictionary<int, Item> ItemType;
    public int Count { get; protected set; }
    public int MaxCount { get; protected set; }
    /// <summary>
    /// How many frame must pass until an item is used
    /// Runs at 100 frames per second. So 20 firerate = 5 uses per second
    /// </summary>
    public int Firerate { get; protected set; } 
    public Item(int count = 1)
    {
        Count = count;
        MaxCount = DefaultMaxCount;
        Firerate = DefaultItemFirerate;
        SetDefaults();
        ModifyCount(0);
    }
    public bool UsePrimary(Player player)
    {
        return OnPrimaryUse(player);
    }
    public bool UseSecondary(Player player)
    {
        return OnSecondaryUse(player);
    }
    /// <summary>
    /// Run once when an item is initialized
    /// Use this to set important item related stats
    /// </summary>
    public virtual void SetDefaults()
    {

    }
    /// <summary>
    /// Run when a held item is left clicked.
    /// Return true on a successful use. 
    /// Defaults to false. 
    /// </summary>
    public virtual bool OnPrimaryUse(Player player)
    {
        return false;
    }
    /// <summary>
    /// Run when a held item is right clicked.
    /// Return true on a successful use. 
    /// Defaults to false,
    /// </summary>
    public virtual bool OnSecondaryUse(Player player)
    {
        return false;
    }
    /// <summary>
    /// Whether or not an item should be consumed upon successfully being used.
    /// Defaults to false
    /// </summary>
    /// <returns></returns>
    public virtual bool IsConsumedOnUse(Player player)
    {
        return false;
    }
    /// <summary>
    /// Whether or not an item can be used by holding fire instead of clicking.
    /// Defaults to true
    /// </summary>
    public virtual bool CanBeAutoReuse(Player player)
    {
        return true;
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
    public void SetCount(int count)
    {
        Count = count;
        if (Count > MaxCount)
            Count = MaxCount;
        if (Count <= 0)
        {
            Count = 0;
        }
    }
}
