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
        for(int i = 0; i < size; i++)
        {
            Set(i, new NoItem());
        }
    }
    public void Set(int i, Item item)
    {
        this.item[i] = item;
    }
    public Item Get(int i)
    {
        return item[i];
    }
    public bool AddItem(Item itemToAdd)
    {
        for (int i = 0; i < Count; i++)
        {
            Item iItem = item[i];
            if(iItem is NoItem)
            {
                item[i] = itemToAdd;
                return true;
            }
        }
        return false;
    }
}
