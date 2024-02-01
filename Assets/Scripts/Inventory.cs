using System;
///Team members that contributed to this script: Ian Bunnell
[Serializable]
public class Inventory //This class exists so that the UI for displaying an item slot could be used in other places in the future.
{
    private Item[] item;
    public int Count { get; private set; }
    public Inventory(int size)
    {
        item = new Item[size];
        Count = size;
    }
    public void Set(int i, Item item)
    {
        this.item[i] = item;
    }
    public Item Get(int i)
    {
        return item[i];
    }
}
