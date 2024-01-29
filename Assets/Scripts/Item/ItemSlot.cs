using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Entity Owner;
    [SerializeField] private int Index;
    [SerializeField] private Text TextElement;
    [SerializeField] private Image ImageElement;
    private Inventory Inventory => Owner.Inventory; //In case the owners inventory is changed, this will be using a function instead of a reference.
    public Item Item => Inventory.Get(Index);
    public bool IsSelected = false;
    private void Update()
    {
        IsSelected = false;
        if (Owner is Player p)
        {
            if(p.SelectedItem == Index)
            {
                IsSelected = true;
            }
        }
        ImageElement.fillCenter = IsSelected;
        TextElement.text = Item.GetType().ToString() + "\n" + Item.Count;
    }
}
