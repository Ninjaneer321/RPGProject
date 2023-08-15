using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    public class ItemGiver : MonoBehaviour
    {
        public InventoryItem item;

        public void GiveItem()
        {
            //Inventory playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>().AddToFirstEmptySlot(item, 1);
            bool success = GameObject.FindWithTag("Player").GetComponent<Inventory>().AddToFirstEmptySlotInventory(item, 1);
            if (!success)
            {
                GetComponent<ItemDropper>().DropItem(item);
            }
        }

    }
}
