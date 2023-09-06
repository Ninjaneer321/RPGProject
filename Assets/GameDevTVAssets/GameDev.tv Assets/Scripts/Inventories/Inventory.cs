using System;
using UnityEngine;
using GameDevTV.Saving;
using GameDevTV.Utils;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using System.Collections;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] public int inventorySize = 16;

        // STATE
        public InventorySlot[] slots;


        public struct InventorySlot
        {
            public InventoryItem item;
            public int number;
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action inventoryUpdated;



        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindInventorySlot(item) >= 0;
        }

        public bool HasSpaceFor(IEnumerable<InventoryItem> items)
        {
            int freeSlots = FreeSlots();
            List<InventoryItem> stackedItems = new List<InventoryItem>();
            foreach (var item in items)
            {
                // Already have it inventory
                if (item.IsStackable())
                {
                    if (HasItem(item)) continue;
                    if (stackedItems.Contains(item)) continue;
                    stackedItems.Add(item);
                }
                // Already seen in the list
                if (freeSlots <= 0) return false;
                freeSlots--;
            }
            return true;
        }

        public int FreeSlots()
        {
            int count = 0;
            foreach (InventorySlot slot in slots)
            {
                if (slot.number == 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            if (!gameObject.CompareTag("Player"))
            {
                return GetComponent<OtherInventorySpawner>().numberOfSlotsToSpawn;
            }      
            return slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlotInventory(InventoryItem item, int number)
        {

            foreach (var store in GetComponents<IItemStore>())
            {
                number -= store.AddInventoryItems(item, number);
            }

            if (number <= 0) return true;

            int i = FindInventorySlot(item);

            if (i < 0)
            {
                return false;
            }

            slots[i].item = item;
            slots[i].number += number;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

            return true;
        }



        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }
        

        public int GetItemSlot(InventoryItem item, int number)
        {
            // Loop through all of the inventory slots. 
            for (int i = 0; i < slots.Length; i++)
            {
                // Check if the current iteration's item equals to the parameter item.
                // Check if the current iteration's item number equals to or greater than the parameter number.
                if (object.ReferenceEquals(slots[i].item, item) && GetNumberInSlot(i) >= number)
                {
                    // If true, return the slot number
                    return i;
                }
            }
            // Return -1 by default.
            // When using this method to check if a slot is found, we simply check if the returned value equals to or greater than 0.
            return -1;
        }

        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;
            if (slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].item = null;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (slots[slot].item != null)
            {
                return AddToFirstEmptySlotInventory(item, number); 
            }

            var i = FindInventoryStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            slots[slot].item = item;
            slots[slot].number += number;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();

            }

            return true;

        }
        public void RemoveItem(InventoryItem item, int number)
        {
            if (item == null) return;
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    slots[i].number -= number;
                    if (slots[i].number <= 0)
                    {
                        slots[i].item = null;
                        slots[i].number = 0;
                    }
                    if (inventoryUpdated != null)
                    {
                        inventoryUpdated();
                    }
                    return;
                }
            }
        }

        public void RemoveItem(string itemID, int number)
        {
            InventoryItem item = InventoryItem.GetFromID(itemID);
            RemoveItem(item, number);
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }
        // PRIVATE


        private void Awake()
        { 
                slots = new InventorySlot[inventorySize];
        }

        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        public int FindInventorySlot(InventoryItem item)
        {
            int i = FindInventoryStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }


        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        public int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <returns>-1 if no stack exists or if the item is not stackable.</returns>
        public int FindInventoryStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }
    
        object ISaveable.CaptureState()
        {
            var slotStrings = new InventorySlotRecord[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].item != null)
                {
                    slotStrings[i].itemID = slots[i].item.GetItemID();
                    slotStrings[i].number = slots[i].number;
                }
            }
            return slotStrings;
        }

        void ISaveable.RestoreState(object state)
        {
            var slotStrings = (InventorySlotRecord[])state;
            for (int i = 0; i < inventorySize; i++)
            {
                slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
                slots[i].number = slotStrings[i].number;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasItem:
                    return HasItem(InventoryItem.GetFromID(parameters[0]));
                case EPredicate.HasItems:
                    InventoryItem item = InventoryItem.GetFromID(parameters[0]);
                    int stack = FindInventoryStack(item);
                    if (stack == -1) return false;
                    if (int.TryParse(parameters[1], out int result))
                    {
                        return slots[stack].number >= result;
                    }
                    return false;
            }
            return null;
        }

        public bool HasItems(InventoryItem item, int number)
        {
            int stack = FindInventoryStack(item);
            if (stack == -1) return false;
            return slots[stack].number >= number;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return true;
                }
            }
            return false;
        }

    }
}