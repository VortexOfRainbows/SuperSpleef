using System;
using UnityEditor;
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
        bool addedItem = false;
        int count = itemToAdd.Count;
        for(int j = 0; j <= 1; j++)
        {
            for (int i = 0; i < Count; i++)
            {
                Item iItem = item[i];
                if(j == 1)
                {
                    if (iItem is NoItem)
                    {
                        item[i] = itemToAdd;
                        item[i].SetCount(count);
                        return true;
                    }
                }
                else
                {
                    if(iItem.GetType() == itemToAdd.GetType())
                    {
                        if(itemToAdd is PlaceableBlock P && iItem is PlaceableBlock PP)
                        {
                            if (P.PlaceID != PP.PlaceID)
                                continue;
                        }
                        item[i].ModifyCount(count);
                        if (item[i].Count > item[i].MaxCount)
                        {
                            int over = item[i].MaxCount - item[i].Count;
                            count = over;
                            addedItem = true;
                        }
                        else
                            return true;
                    }
                }
            }
        }
        return addedItem;
    }
}
