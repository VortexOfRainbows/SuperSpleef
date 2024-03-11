using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    public int ItemIndex => Index;
    [SerializeField] private InventoryModel Model;
    [SerializeField] private Entity Owner;
    [SerializeField] private int Index;
    [SerializeField] private Text TextElement;
    [SerializeField] private Image ImageElement;
    private Inventory Inventory => Owner.Inventory; //In case the owners inventory is changed, this will be using a function instead of a reference.
    public Item Item => Inventory.Get(Index);
    private System.Type lastType = null;
    private int lastCount = -1;
    public bool IsSelected = false;
    private void Update()
    {
        if(Owner == null)
        {
            foreach(Player player in GameStateManager.Players)
            {
                if(player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    Owner = player;
                }
            }
            return;
        }
        IsSelected = false;
        if (Owner is Player p)
        {
            if(p.SelectedItem == Index)
            {
                IsSelected = true;
            }
        }
        ImageElement.fillCenter = IsSelected; 
        if (lastCount != Item.Count || lastType != Item.GetType())
        {
            ReloadVisuals();
        }
    }
    /// <summary>
    /// Reloads the display text and 3d block model associated with the inventory item
    /// </summary>
    private void ReloadVisuals()
    {
        bool isNotItem = Item is NoItem;
        if (!isNotItem)
            Model.gameObject.SetActive(true);
        System.Type itemType = Item.GetType();
        string str = itemType.ToString().AddSpaceBetweenCaps(); //Using System.Type is a temporary solution until we use localization or other means of naming items
        if (Item.Count > 0 && !isNotItem)
        {
            str += "\n" + Item.Count;
        }
        TextElement.text = str;
        lastCount = Item.Count;
        lastType = itemType;
        if(Item is PlaceableBlock pb)
        {
            Model.SetModelToBlock(pb.PlaceID);
        }
        else
        {
            if(isNotItem)
            {
                Model.gameObject.SetActive(false);
            }
            else
                Model.SetModelToBlock(BlockID.Air);
        }
    }
}
