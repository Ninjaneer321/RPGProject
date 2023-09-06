using UnityEngine;
using GameDevTV.Inventories;
using GameDevTV.Core.UI.Dragging;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using RPG.Stats;
using RPG.Combat;
using RPG.Abilities;

namespace GameDevTV.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;


        // STATE
        int index;
        Inventory inventory;
        InventoryUI inventoryUI;


        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }
        public Ability GetAbility()
        {
            Debug.Log("Not implemented");
            throw new System.NotImplementedException();
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }

        public void ItemClicked()
        {
            Debug.Log("ItemClicked");
            if (GetItem() == null || GetNumber() < 1) return;

            if (!gameObject.GetComponentInParent<InventoryUI>().isPlayerInventory)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Inventory playerInventory = player.GetComponent<Inventory>();
                playerInventory.AddToFirstEmptySlotInventory(GetItem(), GetNumber());

                string newItemString = "<br>Item received: " + GetItem().GetDisplayName() + ". x:" + GetNumber() + ".";
                ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
                chatBox.UpdateText(newItemString);

                RemoveItems(GetNumber());


                //Destroy(gameObject);
                //Health nonPlayer = player.GetComponent<Fighter>().GetTarget();
                //Inventory nonPlayerInventory = nonPlayer.GetComponent<Inventory>();
                //Debug.Log(nonPlayerInventory.GetItemSlot(GetItem(), index));
                //nonPlayerInventory.RemoveFromSlot(nonPlayerInventory.GetItemSlot(GetItem(), index), GetNumber());
                //nonPlayerInventory.RemoveItem(GetItem(), GetNumber());
                //Add item to players inventory

                return;
            }

            if (GetItem() is EquipableItem equipableItem)
                {
                    Equipment equipment = inventory.GetComponent<Equipment>();
                    if (equipableItem.CanEquip(equipableItem.GetAllowedEquipLocation(), equipment))
                    {
                        EquipableItem equippedItem =
                        equipment.GetItemInSlot(equipableItem.GetAllowedEquipLocation());
                        equipment.AddItem(equipableItem.GetAllowedEquipLocation(), equipableItem);
                        RemoveItems(1);
                        if (equippedItem != null) AddItems(equippedItem, 1);
                    }
                }
        }


    }
}